using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using BatisAutomationWebApi.dtos;
using BatisAutomationWebApi.Utilities;
using BatissWebOA.Presentation.Utility;
using DataTransferObjects;
using DataTransferObjects.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var forms =  (await EnterpriseFormsService.GetLetterOwnerEnterpriseForms(owner)).ToList();
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
            var form =  await EnterpriseFormsService.GetEnterpriseForm(request.FormId);
            foreach (var bookmark in form.Bookmarks)
            {
                bookmark.DefaultValue =  await ParametersUtility.UpdateParameters(bookmark.DefaultValue, request.OwnerId);
            }
            return form;
        }

        [Route("FormValidValues")]
        public  EnterpriseFormVaildValuesForTableColumnDto Post([FromBody] EnterpriseFormValidValuesRequest request)
        {
            return LetterService.GetFormValidValues(request.FormId);
        }

        [Route("BehindCodeExecutionResult")]
        public async Task<JsonResult<string>> Post([FromBody] BehindCodeCompilingRequest request)
        {
            try
            {
                var formInfo = await CodeBehindCompiler.GetEnterpriseFormInfo(request.FormId.ToString(), request.ParametersValue, request.TableParametersValue);
                var validationResult =  await CodeBehindCompiler.GetFormValidatorResult(request.FormId.ToString(),
                    formInfo.EnterpriseForm.ValidationComputationCode, formInfo.ParametersValueDictunary,
                    formInfo.TableParameterRows, request.ChangedParameterName, request.OwnerId.ToString());

                if (validationResult != null && !validationResult.HasError && !validationResult.HasBusinessError)
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
                        var reloadedData = await CodeBehindCompiler.ReloadTableData(request.FormId,identificationInfo,validationResult.RequestForReloadingTableValues);
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

                    var result = jObj.ToString();
                    return Json(result);

                }
                else
                {
                    if (validationResult != null)
                    {
                        var jObj = new JObject();
                        jObj["Errors"] = JsonConvert.SerializeObject(validationResult.Errors);
                        return Json(jObj.ToString());
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
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