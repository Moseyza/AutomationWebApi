using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BatisAutomationWebApi.dtos;
using DataTransferObjects;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/AnnouncementBoards")]
    public class AnnouncementBoardController : ControllerBase
    {
        [Route("AllAccessible")]
        public async Task<IList<AnnouncementBoardDto>> Post([FromBody] AccessibleAnnouncementBoardsRequest request)
        {
            var ownerDto =  await LetterOwnerService.GetOwnerDto(request.LetterOwnerId);
            return  await AnnouncementBoardService.GetAccessibleAnnouncementBoards(ownerDto);
        }


    }
}