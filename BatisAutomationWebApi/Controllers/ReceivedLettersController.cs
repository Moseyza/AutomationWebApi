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
        public async Task<LetterListerWithPaginationResult> Post([FromBody]PaginatedLettersRequestDto1 request)
        {
            
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(request.From))
                from = DateTime.Parse(request.From.Substring(0, request.From.ToUpper().IndexOf("GMT", StringComparison.Ordinal) - 1));
            if (!string.IsNullOrEmpty(request.To))
                to = DateTime.Parse(request.To.Substring(0,request.To.ToUpper().IndexOf("GMT", StringComparison.Ordinal) - 1));
            return await LetterService.GetReceivedLettersWithPagination(request.OwnerId,from,to);
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