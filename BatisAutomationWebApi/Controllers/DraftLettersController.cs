using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    public class DraftLettersController : ControllerBase
    {

        [Authorize]
        public async Task<LetterListerWithPaginationResult> Post([FromBody]PaginatedLettersRequestDto request)
        {

            var result =  await LetterService.GetDraftLettersWithPagination(request.OwnerId, request.From, request.To);
            return result;
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