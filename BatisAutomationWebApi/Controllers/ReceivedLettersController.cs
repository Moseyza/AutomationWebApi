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
    public class ReceivedLettersController : ControllerBase
    {
        

      
        public async Task<LetterListerWithPaginationResult> Post([FromBody]PaginatedLettersRequestDto request)
        {
            return await LetterService.GetReceivedLettersWithPagination(request.OwnerId,request.From,request.To);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}