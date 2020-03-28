using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    public class SentLettersController : ControllerBase
    {
    
        
        public async Task<LetterListerWithPaginationResult> Post([FromBody]PaginatedLettersRequestDto request)
        {
           return  await LetterService.GetSentLettersWithPagination(request.OwnerId,request.From,request.To);
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