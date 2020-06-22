using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/AutoCompleteData")]
    public class AutoComleteDataController : ControllerBase
    {
        
        [Route("Close")]
        public async Task<IEnumerable<AutoCompleteDataDto>>  Post([FromBody]string value)
        {
            return await AutoCompleteDataService.GetCloseAutoCompleteData();
        }


        [Route("Forward")]
        public async Task<IEnumerable<AutoCompleteDataDto>> Post([FromBody]ForwardingAutoCompleteDataRequest request)
        {
            return await AutoCompleteDataService.GetAllForwardingAutoCompleteData();
        }
        [Route("SendCopy")]
        public async Task<IEnumerable<AutoCompleteDataDto>> Post([FromBody] SendCopyAutoCompleteDataRequest request)
        {
            return await AutoCompleteDataService.GetSendCopyAutoCompleteData();
        }

        [Route("SendDraft")]
        public  IEnumerable<AutoCompleteDataDto> Post([FromBody] SendDraftAutoCompleteDataRequest request)
        {
            return  AutoCompleteDataService.GetSendDraftAutoCompleteData();
        }


    }
}