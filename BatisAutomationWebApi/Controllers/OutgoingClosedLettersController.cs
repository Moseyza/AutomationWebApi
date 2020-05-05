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
    public class OutgoingClosedLettersController : ControllerBase
    {

        // POST api/<controller>
        public async Task<LetterListerWithPaginationResult> Post([FromBody]PaginatedLettersRequestDto dto)
        {
            return await LetterService.GetOutgoingClosedLetters(dto.OwnerId,dto.From,dto.To);
        }

      
    }
}