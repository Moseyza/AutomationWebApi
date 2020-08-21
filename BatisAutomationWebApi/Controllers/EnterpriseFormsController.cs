using BatisAutomationWebApi.dtos;
using BatisAutomationWebApi.Utilities;
using BatissWebOA.Presentation.Utility;
using DataTransferObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using LetterOwnerDto = DataTransferObjects.LetterOwnerDto;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/EnterpriseForms")]
    public class EnterpriseFormsController : ControllerBase
    {
        [Route("UsedForms")]
        public async Task<IEnumerable<EnterpriseFormDto>> Post([FromBody] LetterOwnerDto dto)
        {
            return await EnterpriseFormsService.GetOwnerUsedEnterpriseForms(dto.Id);
        }


        [Route("OwnerForms")]
        public async Task<IEnumerable<EnterpriseFormDto>> Post([FromBody] OwnerFormsRequest request)
        {
            var owner = await LetterOwnerService.GetOwnerDto(request.OwnerId);
            var forms = (await EnterpriseFormsService.GetLetterOwnerEnterpriseForms(owner)).ToList();
            foreach (var form in forms)
            {
                foreach (var bookmark in form.Bookmarks)
                {
                    bookmark.DefaultValue = await ParametersUtility.UpdateParameters(bookmark.DefaultValue, request.OwnerId);
                }
            }

            return forms;
        }


        [Route("Form")]
        public async Task<EnterpriseFormDto> Post([FromBody] EnterpriseFormRequest request)
        {
            var form = await EnterpriseFormsService.GetEnterpriseForm(request.FormId);
            foreach (var bookmark in form.Bookmarks)
            {
                bookmark.DefaultValue = await ParametersUtility.UpdateParameters(bookmark.DefaultValue, request.OwnerId);
            }
            return form;
        }

        [Route("FormValidValues")]
        public EnterpriseFormVaildValuesForTableColumnDto Post([FromBody] EnterpriseFormValidValuesRequest request)
        {
            return LetterService.GetFormValidValues(request.FormId);
        }

        [Route("FormReceivers")]
        public async Task<IEnumerable<LetterOwnerDtoWithFaxAndEmails>> Post([FromBody] FormReceiversRequest request)
        {
            try
            {
                var result = await LetterOwnerService.GetEnterpriseFormReceivers(request.FormId, request.SenderId, request.DependentLetterId);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Route("BehindCodeExecutionResult")]
        public async Task<JsonResult<string>> Post([FromBody] BehindCodeCompilingRequest request)
        {
            try
            {
                var formInfo = await CodeBehindCompiler.GetEnterpriseFormInfo(request.FormId.ToString(), request.ParametersValue, request.TableParametersValue);
                var validationResult = await CodeBehindCompiler.GetFormValidatorResult(request.FormId.ToString(),
                    formInfo.EnterpriseForm.ValidationComputationCode, formInfo.ParametersValueDictunary,
                    formInfo.TableParameterRows, request.ChangedParameterName, request.OwnerId.ToString());

                if (validationResult != null /*&& !validationResult.HasError && !validationResult.HasBusinessError*/)
                {
                    var newValueList = new List<dynamic>();
                    var identificationInfo = await CodeBehindCompiler.GetAccountInfo(request.OwnerId);
                    foreach (var item in validationResult.NewValues)
                    {
                        dynamic obj = new ExpandoObject();
                        obj.Name = item.Key;
                        obj.Value = item.Value;
                        newValueList.Add(obj);
                    }

                    if (validationResult.RequestForReloadingTableValues.Count > 0)
                    {
                        var reloadedData = await CodeBehindCompiler.ReloadTableData(request.FormId, identificationInfo, validationResult.RequestForReloadingTableValues);
                        foreach (var table in reloadedData.Keys)
                        {
                            var value = reloadedData[table];
                            dynamic dynamicCollection = System.Web.Helpers.Json.Decode(value);
                            var tempDynamicList = new List<dynamic>();
                            foreach (var item in dynamicCollection)
                            {
                                tempDynamicList.Add(item);
                            }
                            formInfo.TableParameterRows[table] = tempDynamicList;
                        }
                    }
                    var serializer = new JavaScriptSerializer();
                    serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });
                    var json = serializer.Serialize(newValueList);
                    var tableParameterRowJson = Json(JsonConvert.SerializeObject(formInfo.TableParameterRows));
                    var jObj = new JObject();
                    jObj["NewValues"] = json;
                    jObj["TableParameterRows"] = tableParameterRowJson.Content;
                    if (validationResult.HasError || validationResult.HasBusinessError)
                    {
                        jObj["HasError"] = true;
                        var errors = "";
                        foreach (var error in validationResult.Errors)
                        {
                            if (string.IsNullOrEmpty(error)) errors = error;
                            else errors += '\n' + error;
                        }

                        jObj["Errors"] = errors;
                    }
                    var result = jObj.ToString();
                    return Json(result);

                }
                else
                {
                    //if (validationResult != null)
                    //{
                    //    var jObj = new JObject();
                    //    jObj["Errors"] = JsonConvert.SerializeObject(validationResult.Errors);
                    //    return Json(jObj.ToString());
                    //}
                }

                return Json("");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [Route("ClientSideInitialize")]
        public async Task<JsonResult<string>> Post([FromBody] ClientSideInitializeRequest request)
        {
            var JsonArray = new JArray();
            var resultJObj = new JObject();
            var formInfo = await CodeBehindCompiler.GetEnterpriseFormInfo(request.FormId.ToString(), request.ParametersValue, request.TableParametersValue);
            foreach (var parameterName in formInfo.ParametersValueDictunary.Keys)
            {
                var initializeResult = CodeBehindCompiler.GetClientSideInitializeResults(request.FormId.ToString(), formInfo.EnterpriseForm.ValidationComputationCode, formInfo.ParametersValueDictunary, formInfo.TableParameterRows, parameterName, request.OwnerId.ToString());
                resultJObj[parameterName] = initializeResult;
            }
            var result = Json(resultJObj.ToString());
            return result;
        }

        [Route("Send")]
        public async Task<SentLetterInformationDto> Post([FromBody] SendFormRequest request)
        {

            try
            {
                var formId = request.FormId;
                var senderId = request.SenderId;
                var bookmarks = request.Bookmarks;
                var tableBookmarks = request.TableBookmarks;
                //Getting EnterpriseForm

                var enterpriseForm = await EnterpriseFormsService.GetEnterpriseForm(formId);

                //Creating bookmarks
                dynamic bookmarksDynamic = System.Web.Helpers.Json.Decode(bookmarks);
                var bookmarkDtos = GetBookmarks(bookmarksDynamic);
                var attachedFiles = GetFileBookmarks(bookmarksDynamic);
                foreach (var bookmark in bookmarkDtos)
                {
                    bookmark.Value = ParametersUtility.ReplacePresianNumberCharsInDateStr(bookmark.Value);
                }

                var tableParameterRows = CodeBehindCompiler.GetTableParameterRows(tableBookmarks);
                var tableBookmarksDtos = new List<EnterpriseFormBookmarkValueDto>();
                var jObject = JObject.Parse(tableBookmarks);

                foreach (var tableName in tableParameterRows.Keys)
                {
                    var jsonStr = jObject[tableName].ToString();
                    var tableBookmark = enterpriseForm.Bookmarks.FirstOrDefault(x => x.EnglishName == tableName);
                    var tableId = Guid.Empty;
                    if (tableBookmark != null) tableId = tableBookmark.Id;
                    tableBookmarksDtos.Add(new EnterpriseFormBookmarkValueDto() { EnterpriseFormBookmarkName = tableName, Value = jsonStr, EnterpriseFormBookmarkId = tableId });
                }

                var sendLetterDto = new SendLetterDto() { LetterRefrences = new List<LetterReferencesToOtherLettersDto>(), DateTime = DateTime.Now };
                sendLetterDto.Sender = new LetterOwnerDto() { Id = senderId };
                sendLetterDto.Recievers = request.Receivers;
                sendLetterDto.CopyRecievers = request.CopyReceivers;
                sendLetterDto.DraftRecievers = request.DraftReceivers;
                //setting some properties
                sendLetterDto.TransferType = TransferType.SystemDelivery;
                sendLetterDto.Title = await ParametersUtility.UpdateParameters(enterpriseForm.Title, senderId);
                sendLetterDto.Title = ReplaceEnglishNameWithValue(bookmarkDtos, sendLetterDto.Title);
                sendLetterDto.Abstract = await ParametersUtility.UpdateParameters(enterpriseForm.Abstract, senderId);
                sendLetterDto.Abstract = ReplaceEnglishNameWithValue(bookmarkDtos, sendLetterDto.Abstract);
                sendLetterDto.IsSecured = enterpriseForm.IsSecured;
                sendLetterDto.Priority = enterpriseForm.Priority;
                var letterName = await ParametersUtility.UpdateParameters(enterpriseForm.MainLetterName, senderId);
                letterName = ReplaceEnglishNameWithValue(bookmarkDtos, letterName);
                var extention = System.IO.Path.GetExtension(enterpriseForm.File.Extension);
                if (string.IsNullOrEmpty(extention))
                    extention = ".docx";
                letterName = letterName + extention;
                //setting parts
                var fileId = enterpriseForm.File.Id;
                var file = await FileService.GetFile(fileId);
                file.Extension = letterName;

                file.Id = Guid.Empty;
                file.IsBasedOnPattern = true;
                sendLetterDto.Parts = new List<PartsDto>() { new PartsDto() { PartIndex = 0, File = file } };

                //Adding Attached files
                //sendLetterDto.Parts.AddRange(GetRequestAttachedFiles(Request, tableBookmarksDtos));
                //Add saved files ides
                //var savedFileIdes = Request["SavedFilesIds"];
                //if (!string.IsNullOrEmpty(savedFileIdes))
                //sendLetterDto.Parts.AddRange(AppendSavedFiles(Request["SavedFilesIds"]));


                //Setting draft parent if exists
                ///var parentDraftId = Request["ParentDraftId"];
                //if (!string.IsNullOrEmpty(parentDraftId))
                //{
                //sendLetterDto.ParentDraftLetterId = new Guid(parentDraftId);
                //}
                //Setting dependent letters if exists
                //Guid dependentLetterId = Guid.Empty;
                //Guid.TryParse(Request["DependentLetterId"], out dependentLetterId);
                //if (dependentLetterId != Guid.Empty)
                //{
                //sendLetterDto.LetterRefrences = new List<LetterReferencesToOtherLettersDto>() { new LetterReferencesToOtherLettersDto() { LetterId = dependentLetterId } };
                //}
                //Merge bookmarks
                bookmarkDtos.AddRange(tableBookmarksDtos);
                //Creating SendEnterpriseFormDto 
                var sendEnterpriseFormDto = new SendEnterpriseFormDto() { EnterpriseFormId = formId };
                sendEnterpriseFormDto.BookmarkValues = bookmarkDtos;
                sendEnterpriseFormDto.SendLetterInformation = sendLetterDto;
                //Mapping file bookmark to parts
                var fileIndex = 0;
                foreach (var part in sendLetterDto.Parts)
                {
                    part.PartIndex = fileIndex++;
                }
                var sendLetterInformation = await LetterService.SendEnterpriseForm(sendEnterpriseFormDto);
                //dynamic letterSendingInfo = new ExpandoObject();
                //letterSendingInfo.Message = "نامه با شماره " + sendLetterInformation.LetterNumber + " ارسال گردید. ";
                //var serializer = new JavaScriptSerializer();
                //serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });
                //var json = serializer.Serialize(letterSendingInfo);
                return sendLetterInformation;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private List<EnterpriseFormBookmarkValueDto> GetBookmarks(dynamic bookmarksDynamic)
        {
            var result = new List<EnterpriseFormBookmarkValueDto>();
            foreach (dynamic obj in bookmarksDynamic)
            {
                try
                {
                    result.Add(new EnterpriseFormBookmarkValueDto() { EnterpriseFormBookmarkName = obj.Name, Value = obj.Value, EnterpriseFormBookmarkId = new Guid(obj.Id.ToString()) });
                }
                catch (Exception e)
                {
                    //ignore
                }

            }
            return result;
        }
        private List<FileDto> GetFileBookmarks(dynamic bookmarksDynamic)
        {
            var result = new List<FileDto>();
            foreach (dynamic obj in bookmarksDynamic)
            {
                try
                {
                    var file = new FileDto() { Extension = obj.Value.Extension, Id = new Guid(obj.Value.Id), Content = Convert.FromBase64String(obj.Value.Content) };
                    result.Add(file);
                }
                catch (Exception e)
                {
                    //ignore
                }

            }
            return result;
        }
        private string ReplaceEnglishNameWithValue(List<EnterpriseFormBookmarkValueDto> bookmarks, string inputString)
        {
            foreach (var bookmark in bookmarks)
            {
                inputString = inputString.Replace("%%" + bookmark.EnterpriseFormBookmarkName + "%%", bookmark.Value);
            }
            return inputString;
        }

    }

    public class ExpandoJSONConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            var dictionary = obj as IDictionary<string, object>;
            foreach (var item in dictionary)
                result.Add(item.Key, item.Value);
            return result;
        }
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new ReadOnlyCollection<Type>(new Type[] { typeof(System.Dynamic.ExpandoObject) });
            }
        }
    }
}