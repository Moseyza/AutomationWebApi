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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using DynamicCodeRunning.ValidationAndCalculationForEnterpriseForms;
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

                foreach (var formBookmark in form.Bookmarks)
                {
                    if(formBookmark.Type != EnterpriseFormBookmarksTypesDto.Table)continue;
                    formBookmark.TableColumns = formBookmark.TableColumns.OrderBy(x => x.ColumnIndex).ToList();
                }
            }

            return forms;
        }
        [Route("UpdateParameters")]
        public async  Task<string> Post([FromBody]UpdateParameterRequest request)
        {
            var result =  await ParametersUtility.UpdateParameters(request.Value, request.OwnerId);
            return result;
        }

        [Route("IsAutoClose/{formId}")]
        public async Task<bool> Get(Guid formId)
        {
            var form = await EnterpriseFormsService.GetEnterpriseForm(formId);
            return form.AutomaticClosing;
        }


        [Route("Form")]
        public async Task<EnterpriseFormDto> Post([FromBody] EnterpriseFormRequest request)
        {
            var form = await EnterpriseFormsService.GetEnterpriseForm(request.FormId);
            foreach (var bookmark in form.Bookmarks)
            {
                bookmark.DefaultValue = await ParametersUtility.UpdateParameters(bookmark.DefaultValue, request.OwnerId);
            }
            foreach (var bookmark in form.Bookmarks)
            {
                if (bookmark.Type == EnterpriseFormBookmarksTypesDto.Table)
                {
                    bookmark.TableColumns = bookmark.TableColumns.OrderBy(x => x.ColumnIndex).ToList();
                }
            }
            return form;
        }

        [Route("NextForm")]
        public async Task<NextEnterpriseFormResponse> Post([FromBody] NextEnterpriseFormRequest request)
        {
            var response = new NextEnterpriseFormResponse();
            var form = await EnterpriseFormsService.GetEnterpriseForm(request.FormId);
            var multipleFormValues = await LetterService.GetMultipleEnterpriseFormValues(request.LetterId);
            foreach (var bookmark in form.Bookmarks)
            {
                bookmark.DefaultValue = await ParametersUtility.UpdateParameters(bookmark.DefaultValue, request.OwnerId);
            }

            if (form.ShallInheritAnnouncementBoard)
            {
                var announcementBoardId = await LetterService.GetAnnouncementBoardIdForLetter(request.LetterId);
                if (announcementBoardId == Guid.Empty)
                    response.AnnouncementBoardIdToSend = announcementBoardId;
                else
                    response.AnnouncementBoardIdToSend = form.DefaultAnnouncementBoardId;
            }

            form.Bookmarks = form.Bookmarks.ToList();
            foreach (var bookmark in form.Bookmarks)
            {
                if (bookmark.Type == EnterpriseFormBookmarksTypesDto.Table)
                {
                    bookmark.TableColumns = bookmark.TableColumns.ToList().OrderBy(x => x.ColumnIndex).ToList();
                }
            }
            response.EnterpriseForm = form; 
            response.DependentLetterId = request.LetterId;
            response.MultipleValues = multipleFormValues;
            response.SenderId = request.OwnerId;

            return response;
        }

        [Route("NextFormsList")]
        public async Task<IEnumerable<EnterpriseFormDto>> Post([FromBody] NextFormsListRequest request)
        {
             return await EnterpriseFormsService.GetNextEnterpriseForms(request.LetterOwnerId, request.LetterId);
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
                    formInfo.TableParameterRows, request.ChangedParameterName, request.OwnerId.ToString(),LetterService);

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
                    serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJsonConverter() });
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

        [Route("ClientSideInitialEvaluate")]
        public async Task<EnterpriseFormValidatorResult> Post([FromBody] ClientSideInitialEvaluateRequest request)
        {
            JArray JsonArray = new JArray();
            JObject resultJObj = new JObject();
            EnterpriseFormInfo formInfo = await CodeBehindCompiler.GetEnterpriseFormInfo(request.FormId.ToString(), request.ParametersValue, request.TableParametersValue);
            //foreach (var parameterName in formInfo.ParametersValueDictunary.Keys)
            //{
                var initializeResult = await CodeBehindCompiler.GetClientSideInitialEvaluateResults(request.FormId.ToString(), formInfo.EnterpriseForm.ValidationComputationCode, formInfo.ParametersValueDictunary, formInfo.TableParameterRows, request.OwnerId.ToString());
                //resultJObj[parameterName] = "";//initializeResult;
            //}
            return initializeResult;
        }

        [Route("LoadDraft")]
        public async Task<LoadDraftEnterpriseFormResponse> Post([FromBody] LoadDraftEnterpriseFormRequest request)
        {
            var temp =  await  LetterService.GetDraftLetter(request.PossessionId);
            var tempList = ( await LetterService.GetAllSavedDraftsWithPagination(request.OwnerId, temp.CreateTime.AddHours(-1),
                temp.CreateTime.AddHours(1))).LetterList;
            var draftLetterDto =  tempList.FirstOrDefault(x => x.Id == request.LetterId);
            var draftEnterpriseForm =  await EnterpriseFormsService.GetBasedOnDraftLetterWithValue(request.LetterId);
            var response = new LoadDraftEnterpriseFormResponse()
            {
                SenderId = request.OwnerId,
                DraftEnterpriseForm = draftEnterpriseForm,
                DraftLetter = draftLetterDto,
                DraftLetterId = request.LetterId

            };
            return response;
        }

        [Route("Send/{mode}")]
        public async Task<SendingFormResults> Post(/*[FromBody] SendFormRequest request*/string mode)
        {
            try
            {
                var receivers = JsonConvert.DeserializeObject<List<LetterOwnerDtoForSendingFaxAndEmailAndSms>>(HttpContext.Current.Request.Params["receivers"]);
                var copyReceivers = JsonConvert.DeserializeObject<List<LetterOwnerDtoForSendingFaxAndEmailAndSms>>(HttpContext.Current.Request.Params["copyReceivers"]);
                var draftReceivers = JsonConvert.DeserializeObject<List<LetterOwnerDtoForSendingFaxAndEmailAndSms>>(HttpContext.Current.Request.Params["draftReceivers"]);
                var formId = new Guid(JsonConvert.DeserializeObject<string>(HttpContext.Current.Request.Params["formId"].ToString()));
                var senderId = new Guid(JsonConvert.DeserializeObject<string>(HttpContext.Current.Request.Params["senderId"].ToString()));
                var bookmarks = JsonConvert.DeserializeObject<string>(HttpContext.Current.Request.Params["bookmarks"]);
                var tableBookmarks = JsonConvert.DeserializeObject<string>(HttpContext.Current.Request.Params["tableBookmarks"]);
                var fileBookmarks = JsonConvert.DeserializeObject<List<string>>(HttpContext.Current.Request.Params["fileBookmarks"]);
                var parentDraftId = Guid.Empty;
                var dependentLetterId = Guid.Empty;
                if(HttpContext.Current.Request.Params["dependentLetterId"] != null)
                    Guid.TryParse(JsonConvert.DeserializeObject<string>(HttpContext.Current.Request.Params["dependentLetterId"].ToString()),out dependentLetterId);
                if (HttpContext.Current.Request.Params["parentDraftId"] != null)
                    Guid.TryParse(JsonConvert.DeserializeObject<string>(HttpContext.Current.Request.Params["parentDraftId"].ToString()), out parentDraftId);


                var enterpriseForm = await EnterpriseFormsService.GetEnterpriseForm(formId);

                //Creating bookmarks
                dynamic bookmarksDynamic = System.Web.Helpers.Json.Decode(bookmarks);
                var bookmarkDtos = GetBookmarks(bookmarksDynamic);
                //var attachedFiles = GetFileBookmarks(bookmarksDynamic);
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

                bool.TryParse(HttpContext.Current.Request.Params["shallSign"], out var shallSign);
                bool.TryParse(HttpContext.Current.Request.Params["shallSetCopyReceivers"], out var shallSetCopyReceivers);
                var sendLetterDto = new SendLetterDto
                {
                    LetterRefrences = new List<LetterReferencesToOtherLettersDto>(),
                    DateTime = DateTime.Now,
                    Sender = new LetterOwnerDto() {Id = senderId},
                    Recievers = receivers,
                    CopyRecievers = copyReceivers,
                    DraftRecievers = draftReceivers,
                    TransferType = TransferType.SystemDelivery,
                    Title = await ParametersUtility.UpdateParameters(enterpriseForm.Title, senderId),
                    ShallSign = shallSign,
                    ShallShowCopyReceiversInLetter = shallSetCopyReceivers
                };
                //setting some properties
                sendLetterDto.Title = await ReplaceEnglishNameWithValue(bookmarkDtos, sendLetterDto.Title,enterpriseForm);
                sendLetterDto.Abstract = await ParametersUtility.UpdateParameters(enterpriseForm.Abstract, senderId);
                sendLetterDto.Abstract = await ReplaceEnglishNameWithValue(bookmarkDtos, sendLetterDto.Abstract, enterpriseForm);
                sendLetterDto.IsSecured = enterpriseForm.IsSecured;
                sendLetterDto.Priority = enterpriseForm.Priority;
                var letterName = await ParametersUtility.UpdateParameters(enterpriseForm.MainLetterName, senderId);
                letterName = await ReplaceEnglishNameWithValue(bookmarkDtos, letterName, enterpriseForm);
                var extension = Path.GetExtension(enterpriseForm.File.Extension);
                if (string.IsNullOrEmpty(extension))
                    extension = ".docx";
                letterName = letterName + extension;

                //setting parts
                var fileId = enterpriseForm.File.Id;
                var mainFile = await FileService.GetFile(fileId);
                mainFile.Extension = letterName;

                mainFile.Id = Guid.Empty;
                mainFile.IsBasedOnPattern = true;
                var formParts = new List<PartsDto>() { new PartsDto() { PartIndex = 0, File = mainFile } };

                //Adding Attached files
                var attachedParts = GetRequestAttachedFiles(HttpContext.Current.Request.Files,fileBookmarks);
                formParts.AddRange(attachedParts);
                sendLetterDto.Parts = formParts;

                //Add saved files ides
                //var savedFileIdes = Request["SavedFilesIds"];
                //if (!string.IsNullOrEmpty(savedFileIdes))
                //sendLetterDto.Parts.AddRange(AppendSavedFiles(Request["SavedFilesIds"]));


                //Setting draft parent if exists

                if (parentDraftId != Guid.Empty) sendLetterDto.ParentDraftLetterId = parentDraftId;
                
                //Setting dependent letters if exists
                //Guid dependentLetterId = Guid.Empty;
                //Guid.TryParse(Request["DependentLetterId"], out dependentLetterId);
                if (dependentLetterId != Guid.Empty)
                {
                    sendLetterDto.LetterRefrences = new List<LetterReferencesToOtherLettersDto>() { new LetterReferencesToOtherLettersDto() { LetterId = dependentLetterId } };
                }
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
                foreach (var bookmark in sendEnterpriseFormDto.BookmarkValues)
                {
                    if (fileBookmarks.Contains(bookmark.EnterpriseFormBookmarkName))
                    {
                        var bookmarkFileId = bookmark.Value;
                        if (sendLetterDto.Parts.Any(x => x.File.PreferedId.ToString() == bookmarkFileId))
                        {
                            var parts = sendLetterDto.Parts.Where(x => x.File.PreferedId.ToString() == bookmarkFileId);
                            var partsDtos = parts as PartsDto[] ?? parts.ToArray();
                            if (partsDtos.Any())
                            {
                                bookmark.FileIndex = partsDtos.ToList()[0].PartIndex;
                            }
                        }
                    }
                }
                
                var sendFormResults = new SendingFormResults();
                try
                {
                    if (mode == "send")
                    {
                        var sendLetterInformation = await LetterService.SendEnterpriseForm(sendEnterpriseFormDto);
                        if (sendLetterInformation.EnterpriseFormValidatorResult.HasError)
                        {
                            sendFormResults.HasError = true;
                            sendFormResults.ErrorMessage =
                                sendLetterInformation.EnterpriseFormValidatorResult.Errors.Aggregate("",
                                    (current, next) =>
                                        current + ", " + next);
                        }
                        else
                        {
                            sendFormResults.HasError = false;
                            sendFormResults.SentLetterInformation = sendLetterInformation;
                        }
                    }
                    else
                    {
                         var savedDraftIds =  await LetterService.SaveEnterpriseFormDraft(sendEnterpriseFormDto);
                         if (!savedDraftIds.Any())
                         {
                             sendFormResults.ErrorMessage = "در هنگام ذخیره پیش نویس خطایی رخ داد";
                             sendFormResults.HasError = true;
                         }

                    }

                }
                catch (Exception e)
                {

                    sendFormResults.HasError = true;
                    sendFormResults.ErrorMessage = e.Message;
                }

                return sendFormResults;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Route("FormValues")]
        public async Task<EnterpriseFormValuesDto> Post([FromBody]EnterpriseFormsValuesRequest request)
        {
            return await LetterService.GetEnterpriseFormValues(request.LetterId, request.LetterPossessionId);
        }

        private List<PartsDto> GetRequestAttachedFiles(HttpFileCollection files,List<string> fileBookmarks)
        {
            var result = new List<PartsDto>();
            var partIndex = 1;
            foreach (var fileBookmark in fileBookmarks)
            {
                var splitted =  fileBookmark.Split('|');
                Guid fileId;
                var fileName = "";
                if (splitted.Length > 3)
                {
                    //attachment is in table row
                    Guid.TryParse(splitted[3],out fileId);
                    fileName = splitted[4];
                }
                else
                {
                    //attachment is a bookmark
                    Guid.TryParse(splitted[1], out fileId);
                    fileName = splitted[2];
                }
                var part = new PartsDto();
                if (files.AllKeys.Any(x => x.Equals(fileId.ToString())))
                {

                    //a new file
                    var keyIndex =  files.AllKeys.ToList().IndexOf(fileId.ToString());
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(files[keyIndex].InputStream))
                    {
                        fileData = binaryReader.ReadBytes(files[keyIndex].ContentLength);
                    }
                    part.File = new FileDto() { Id = Guid.Empty, PreferedId = new Guid(files.Keys[keyIndex]), Extension = files[keyIndex].FileName, Content = fileData };
                }
                else
                {
                    //a saved file
                    if (fileId == Guid.Empty)continue;
                    part.File = new FileDto() { Id = fileId , Extension = fileName };
                }
                part.PartIndex = partIndex++;
                result.Add(part);
            }

            //for (var i = 0; i < files.Count; i++)
            //{
            //    var part = new PartsDto();
            //    byte[] fileData = null;

            //    using (var binaryReader = new BinaryReader(files[i].InputStream))
            //    {
            //        fileData = binaryReader.ReadBytes(files[i].ContentLength);
            //    }
            //    part.File = new FileDto() { Id = Guid.Empty, PreferedId = new Guid(files.Keys[i]), Extension = files[i].FileName, Content = fileData };
            //    part.PartIndex = index++;
            //    result.Add(part);
            //}

            return result;
        }
        private List<EnterpriseFormBookmarkValueDto> GetBookmarks(dynamic bookmarksDynamic)
        {
            var result = new List<EnterpriseFormBookmarkValueDto>();
            foreach (dynamic obj in bookmarksDynamic)
            {
                try
                {
                    result.Add(new EnterpriseFormBookmarkValueDto() { EnterpriseFormBookmarkName = obj.Name, Value = obj.Value.ToString(), EnterpriseFormBookmarkId = new Guid(obj.Id.ToString()) });
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
        private async Task<string> ReplaceEnglishNameWithValue(List<EnterpriseFormBookmarkValueDto> bookmarks, string inputString,EnterpriseFormDto enterpriseForm)
        {
            foreach (var bookmark in bookmarks)
            {
                var currentBookmark =  enterpriseForm.Bookmarks.FirstOrDefault(bm => bm.Id == bookmark.EnterpriseFormBookmarkId);
                if(currentBookmark == null)continue;
                if (currentBookmark.Type == EnterpriseFormBookmarksTypesDto.Company)
                {
                    var allCompanies = await CompanyService.GetAll();
                    var companyId = Guid.Empty;
                    Guid.TryParse(bookmark.Value, out companyId);
                    var company = allCompanies.ToList().FirstOrDefault(c => c.Id == companyId);
                    if (company == null) continue;
                    inputString = inputString.Replace("%%" + bookmark.EnterpriseFormBookmarkName + "%%", company.Name);
                }
                else
                {
                    inputString = inputString.Replace("%%" + bookmark.EnterpriseFormBookmarkName + "%%", bookmark.Value);
                }
            }
            return inputString;
        }

    }

    public class ExpandoJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            if (!(obj is IDictionary<string, object> dictionary)) return result;
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