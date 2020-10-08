using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    public class IncomingClosedLettersController : ControllerBase
    {
        
        public async Task<LetterListerWithPaginationResult> Post([FromBody]PaginatedLettersRequestDto dto)
        {
            var result =  await LetterService.GetIncomingClosedLetters(dto.OwnerId,dto.From,dto.To);
            foreach (var letterDto in result.LetterList)
            {
                letterDto.IsClosed = true;
            }

            return result;
        }

      
    }
}