using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using BatisServiceProvider.Services;
using BatissWebOA.Presentation.Utility;
using DataTransferObjects;
using DynamicCodeRunning.ValidationAndCalculationForEnterpriseForms;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;

namespace BatisAutomationWebApi.Utilities
{
    public class CodeBehindCompiler
    {
        private static Dictionary<FormBehindCode, Assembly> _behindCodeAssemblies = new Dictionary<FormBehindCode, Assembly>();


        public static async Task<EnterpriseFormValidatorResult> GetFormValidatorResult(string formId, string behindCode, Dictionary<string, string> parametersDic, Dictionary<string, List<dynamic>> tableParameterRow, string changedParameterName, string senderId,LetterService letterService)
        {

            var assembly = GetBehindCodeAssembly(new Guid(formId), behindCode);
            if (assembly != null)
            {
                var validatorInstance = GetValidatorInstance(assembly);
                if (validatorInstance != null)
                {

                    try
                    {
                        var accountInfo = await GetAccountInfo(new Guid(senderId));
                        var result = validatorInstance.ClientSideCheck(parametersDic, accountInfo, changedParameterName, tableParameterRow);
                        if (result.RequestForReloadingData.Any())
                        {
                            var reloadedData =  await letterService.ReloadFormData(new Guid(formId), accountInfo, result.RequestForReloadingData);
                            result =  validatorInstance.ClientSideDataReloader(parametersDic, accountInfo, reloadedData, tableParameterRow);
                        }
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return new EnterpriseFormValidatorResult() { HasError = true, Errors = new List<string>() { ex.Message } };

                    }


                }
                return null;
            }
            return null;
        }

        public static string GetClientSideInitializeResults(string formId, string behindCode, Dictionary<string, string> parametersDic, Dictionary<string, List<dynamic>> tableParameterRow, string parameterName, string senderId)
        {
            var assembly = GetBehindCodeAssembly(new Guid(formId), behindCode);
            if (assembly != null)
            {
                
                IEnterpriseFormValidator validatorInstance = GetValidatorInstance(assembly);
                if (validatorInstance != null)
                {

                    try
                    {
                        var result =
                        validatorInstance.ClientSideInitialize(() => ServerSideInitialize(new Guid(formId), new Guid(senderId), parameterName)).Result;
                        
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return "";

                    }
                }
                return null;
            }
            return null;

        }

        public static async Task<EnterpriseFormValidatorResult> GetClientSideInitialEvaluateResults(string formId, string behindCode, Dictionary<string, string> parametersDic, Dictionary<string, List<dynamic>> tableParameterRow, string senderId)
        {
            var assembly = GetBehindCodeAssembly(new Guid(formId), behindCode);
            if (assembly != null)
            {

                IEnterpriseFormValidator validatorInstance = GetValidatorInstance(assembly);
                if (validatorInstance != null)
                {

                    try
                    {
                        var accountInfo =  await GetAccountInfo(new Guid(senderId));
                        var result =  validatorInstance.ClientSideInitialEvaluate(parametersDic, accountInfo, tableParameterRow);



                        return result;
                    }
                    catch (Exception ex)
                    {
                        return null;

                    }
                }
                return null;
            }
            return null;

        }

        private static IEnterpriseFormValidator GetValidatorInstance(Assembly assembly)
        {
            var types = assembly.GetTypes();
            Type type = null;
            foreach (var t in types)
            {
                if (t.IsSubclassOf(typeof(IEnterpriseFormValidator)))
                {
                    type = t;
                    break;
                }

            }
            if (type != null)
            {
                var instance = (IEnterpriseFormValidator)assembly.CreateInstance(type.FullName);
                return instance;
            }
            return null;
            //var tobj = 
            //var assemblyName = System.IO.Path.GetDirectoryName(typeof(CodeBehindCompiler).Assembly.Location) + "\\" + "test.dll";
            //var type = assembly.GetType("IEnterpriseFormValidator");
            // var types = assembly.GetTypes();
        }
        private static Assembly GetBehindCodeAssembly(Guid formId, string behindCode)
        {
            var formBehindCode = _behindCodeAssemblies.Keys.FirstOrDefault(x => x.FormId == formId);
            if (formBehindCode != null && formBehindCode.BehindCode == behindCode)
            {
                formBehindCode.LastRequestDate = DateTime.Now;
                return _behindCodeAssemblies[formBehindCode];
            }
            var assebly = CompileBehindCode(behindCode);
            AddToAsseblies(assebly, formId, behindCode);
            return assebly;

        }
        private static void AddToAsseblies(Assembly assebly, Guid formId, string behindCode)
        {
            var formBehindCode = _behindCodeAssemblies.Keys.FirstOrDefault(x => x.FormId == formId);
            if (formBehindCode != null)
            {
                formBehindCode.BehindCode = behindCode;
                formBehindCode.LastRequestDate = DateTime.Now;
                _behindCodeAssemblies[formBehindCode] = assebly;
            }
            else
            {
                _behindCodeAssemblies.Add(new FormBehindCode() { FormId = formId, BehindCode = behindCode, LastRequestDate = DateTime.Now }, assebly);
            }
        }
        private static Assembly CompileBehindCode(string behindCode)
        {
            var codeProvider = new CSharpCodeProvider();
            var compiler = codeProvider.CreateCompiler();
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            var parameters = new CompilerParameters(new[] { "System.dll", "System.Data.dll", "mscorlib.dll", "System.Core.dll" });
            parameters.ReferencedAssemblies.Add(path + "\\DynamicCodeRunning.dll");
            parameters.ReferencedAssemblies.Add(path + "\\Microsoft.CSharp.dll");
            parameters.ReferencedAssemblies.Add(path + "\\Newtonsoft.Json.dll");
            parameters.ReferencedAssemblies.Add(path + "\\NPOI.dll");
            parameters.ReferencedAssemblies.Add(path + "\\NPOI.OOXML.dll");
            parameters.ReferencedAssemblies.Add(path + "\\NPOI.OpenXml4Net.dll");
            parameters.ReferencedAssemblies.Add(path + "\\NPOI.OpenXmlFormats.dll");
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            try
            {
                var results = compiler.CompileAssemblyFromSource(parameters, behindCode);
                var assembly = results.CompiledAssembly;
                return assembly;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private static LetterOwnerIdentificationInformationDto _identificationInfo = null;
        public static async Task<Dictionary<string, string>> GetAccountInfo(Guid ownerId)
        {
            var service = new LetterOwnerService();
            if(_identificationInfo == null || _identificationInfo.LetterOwnerId != ownerId.ToString())
                _identificationInfo = await service.GetLetterOwnerIdentificationInformation(ownerId);
            var accountInfo = new Dictionary<string, string>
            {
                {"CompanyContactorName", _identificationInfo.CompanyContactorName},
                {"CompanyName", _identificationInfo.CompanyName},
                {"Department", _identificationInfo.Department},
                {"LastName", _identificationInfo.LastName},
                {"Name", _identificationInfo.Name},
                {"NationalCode", _identificationInfo.NationalCode},
                {"OrganizationalLevel", _identificationInfo.OrganizationalLevel},
                {"PersonnelCode", _identificationInfo.PersonnelCode},
                {"Post", _identificationInfo.Post},
                {"LetterOwnerId", _identificationInfo.LetterOwnerId}
            };
            return accountInfo;
        }

        public static async Task<Dictionary<string, string>> ReloadTableData(Guid id, Dictionary<string, string> accountInfo, IDictionary<string, IDictionary<string, string>> evaluationResultRequestForReloadingTableValues)
        {
           var service = new LetterService();
           return await service.ReloadTableData(id, accountInfo, evaluationResultRequestForReloadingTableValues);
        }

        public static async Task<string> ServerSideInitialize(Guid formId, Guid senderId, string parameterName)
        {
            var service = new LetterService();
            return await service.ServerSideInitialize(formId, await GetAccountInfo(senderId), parameterName);

        }
      

        public static async Task<EnterpriseFormInfo> GetEnterpriseFormInfo(string formId, string parameterValues, string tableParameterValues)
        {
            var enterpriseFormService = new EnterpriseFormsService();
            var enterpriseForm = await enterpriseFormService.GetEnterpriseForm(new Guid(formId));
            var parameterValuesDictionary = GetParametersDictionary(parameterValues);
            var tableParameterRows = GetTableParameterRows(tableParameterValues);
            var tableNames = tableParameterRows.Keys.ToList();
            foreach (var tableName in tableNames)
            {
                var rows = tableParameterRows[tableName];
                foreach (dynamic row in rows)
                {
                    var dic = row as IDictionary<string, object>;
                    foreach (var propertyName in dic.Keys.ToList())
                    {
                        if (!(dic[propertyName] is string))
                        {
                            var dynamicProperty = dic[propertyName] as IDictionary<string, dynamic>;
                            if (dynamicProperty != null)
                                if (dynamicProperty.Keys.Contains("SqlQuery"))
                                {

                                    var selectedValueId = dynamicProperty["SelectedValue"].Id;
                                    var selectedValueVal = dynamicProperty["SelectedValue"].Value;
                                    var sqlQuery = dynamicProperty["SqlQuery"];
                                    dic[propertyName] = new DynamicParamsClass(sqlQuery)
                                    {
                                        SelectedValue =
                                            new IdValuePairClass() { Id = selectedValueId, Value = selectedValueVal }
                                    };
                                }
                                else
                                {
                                    if (dynamicProperty.Keys.Contains("Value") && dynamicProperty.Keys.Contains("Id"))
                                    {
                                        var id = dynamicProperty["Id"];
                                        var value = dynamicProperty["Value"];
                                        dic[propertyName] = new IdValuePairClass() { Id = id, Value = value };
                                    }
                                }
                        }

                    }
                }
            }
            var result = new EnterpriseFormInfo()
            {
                EnterpriseForm = enterpriseForm,
                ParametersValueDictunary = parameterValuesDictionary,
                TableParameterRows = tableParameterRows
            };
            return result;
        }

        public static Dictionary<string, List<dynamic>> GetTableParameterRows(string tableParameterValues)
        {
            
            var tableParameterRows = new Dictionary<string, List<dynamic>>();
            var dynamicSerializer = new JavaScriptSerializer();
            dynamicSerializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic deserilizedData = dynamicSerializer.Deserialize(tableParameterValues, typeof(object));
            var dictionary = (Dictionary<string, object>)GetProperty(deserilizedData);
            var tableNames = dictionary.Select(x => x.Key);
            foreach (var tableName in tableNames)
            {
                var filteredDic = dictionary.Where(x => x.Key == tableName).ToDictionary(x => x.Key, y => y.Value);
                var dynamicList = new List<dynamic>();
                foreach (ArrayList item in filteredDic.Values)
                {
                    foreach (var i in item)
                    {
                        dynamic d = (i as Dictionary<string, object>).ToExpando();
                        dynamicList.Add(d);
                    }
                }
                tableParameterRows.Add(tableName, dynamicList);
            }
            return tableParameterRows;
        }
        public static object GetProperty(object o)
        {
            if (o == null) throw new ArgumentNullException("o");

            Type scope = o.GetType();
            IDynamicMetaObjectProvider provider = o as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                ParameterExpression param = Expression.Parameter(typeof(object));
                DynamicMetaObject mobj = provider.GetMetaObject(param);
                GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, "Dictionary", scope, new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(0, null) });
                DynamicMetaObject ret = mobj.BindGetMember(binder);
                BlockExpression final = Expression.Block(
                    Expression.Label(CallSiteBinder.UpdateLabel),
                    ret.Expression
                );
                LambdaExpression lambda = Expression.Lambda(final, param);
                Delegate del = lambda.Compile();
                return del.DynamicInvoke(o);
            }
            else
            {

            }
            return null;
        }
        private static Dictionary<string, string> GetParametersDictionary(string parameterValues)
        {
            var result = new Dictionary<string, string>();
            try
            { 
                dynamic temp = System.Web.Helpers.Json.Decode(parameterValues);
                foreach (dynamic item in temp)
                {
                    try
                    {
                        result.Add(item.Name, ParametersUtility.ReplacePresianNumberCharsInDateStr(item.Value.ToString()));
                    }
                    catch (Exception e)
                    {
                        
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return result;
        }

    }
    internal class FormBehindCode
    {
        public Guid FormId { set; get; }
        public string BehindCode { set; get; }
        public DateTime LastRequestDate { set; get; }
    }
    public class EnterpriseFormInfo
    {
        public EnterpriseFormDto EnterpriseForm { set; get; }
        public Dictionary<string, string> ParametersValueDictunary { set; get; }
        public Dictionary<string, List<dynamic>> TableParameterRows { set; get; }
    }
}