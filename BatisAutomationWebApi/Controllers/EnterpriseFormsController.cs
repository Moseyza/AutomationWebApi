using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BatisAutomationWebApi.dtos;
using BatisAutomationWebApi.Utilities;
using BatissWebOA.Presentation.Utility;
using DataTransferObjects;
using DataTransferObjects.Utilities;
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
        public async Task<string> Post([FromBody] BehindCodeCompilingRequest request)
        {
            try
            {
                var formInfo = await CodeBehindCompiler.GetEnterpriseFormInfo(request.FormId.ToString(), request.ParametersValue, request.TableParametersValue);
                var compileResult =  await CodeBehindCompiler.GetFormValidatorResult(request.FormId.ToString(),
                    formInfo.EnterpriseForm.ValidationComputationCode, formInfo.ParametersValueDictunary,
                    formInfo.TableParameterRows, request.ChangedParameterName, request.OwnerId.ToString());
                //CodeBehindCompiler.GetFormValidatorResult(request.FormId,request.OwnerId,);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        //[Route("VisibleForms")]
        //public async Task<IEnumerable<EnterpriseFormDto>> Post([FromBody] LetterOwnerDto dto)
        //{
            
        //    return await EnterpriseFormsService.GetOwnerUsedEnterpriseForms(dto.Id);
        //}
    }
}