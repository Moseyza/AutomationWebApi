using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DataTransferObjects;

namespace BatisAutomationWebApi.Controllers
{
    public class ServiceTestController : ControllerBase
    {
       

        // POST api/<controller>

        public async Task<EnterpriseFormValuesDto> Post([FromBody]EnterpriseFormsValuesRequest request)
        {
            return await LetterService.GetEnterpriseFormValues(request.LetterId,request.LetterPossessionId);
        }

    }

    public class EnterpriseFormsValuesRequest
    {
        public Guid LetterId { get; set; }
        public Guid LetterPossessionId { get; set; }
    }
}