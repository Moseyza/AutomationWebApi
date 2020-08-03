using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BatisAutomationWebApi.dtos;
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
            return await EnterpriseFormsService.GetLetterOwnerEnterpriseForms(owner);
        }


        [Route("Form")]
        public async Task<EnterpriseFormDto> Post([FromBody] EnterpriseFormRequest request)
        {
            return await EnterpriseFormsService.GetEnterpriseForm(request.FormId);
        }

        //[Route("VisibleForms")]
        //public async Task<IEnumerable<EnterpriseFormDto>> Post([FromBody] LetterOwnerDto dto)
        //{
            
        //    return await EnterpriseFormsService.GetOwnerUsedEnterpriseForms(dto.Id);
        //}
    }
}