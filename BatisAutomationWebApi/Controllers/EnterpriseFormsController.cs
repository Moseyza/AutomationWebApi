using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DataTransferObjects;
using DataTransferObjects.Utilities;

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

        //[Route("VisibleForms")]
        //public async Task<IEnumerable<EnterpriseFormDto>> Post([FromBody] LetterOwnerDto dto)
        //{
            
        //    return await EnterpriseFormsService.GetOwnerUsedEnterpriseForms(dto.Id);
        //}
    }
}