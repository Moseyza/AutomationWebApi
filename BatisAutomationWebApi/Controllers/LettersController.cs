using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/letters")]
    public class LettersController : ControllerBase
    {
        public async Task<IList<LetterDto>> Get(Guid ownerId, DateTime? to, DateTime? from)
        {
            var result = await LetterService.GetLettersWithDateRange(ownerId, from, to);
            return result.LetterList;
        }
        // GET api/<controller>
        [AllowAnonymous]
        [HttpGet]
        [Route("forall")]
        public IHttpActionResult Get()
        {
            return Ok("Now server time is: " + DateTime.Now.ToString());
            
        }

        //[Authorize]
        //[HttpGet]
        //[Route("api/data/authenticate")]
        //public IHttpActionResult GetForAuthenticate()
        //{
        //    var identity = (ClaimsIdentity)User.Identity;
        //    return Ok("Hello " + identity.Name);
        //}

        //[Authorize(Roles = "admin")]
        //[HttpGet]
        //[Route("api/data/authorize")]
        //public IHttpActionResult GetForAdmin()
        //{
        //    var identity = (ClaimsIdentity)User.Identity;
        //    var roles = identity.Claims
        //        .Where(c => c.Type == ClaimTypes.Role)
        //        .Select(c => c.Value);
        //    return Ok("Hello " + identity.Name + " Role: " + string.Join(",", roles.ToList()));
        //}


        // POST api/<controller>
        [Route("OpenLetter")]
        public async Task<OpenLetterResultDto> Post([FromBody]OpenLetterRequestDto request)
        {
            return await LetterService.OpenLetter(request.LetterPossessionId);
        }

        [Route("LetterTrail")]
        public async Task<LetterTrailDto> Post([FromBody]LetterTrailRequestDto request)
        {
            return await LetterService.GetLetterTrail(request.LetterPossessionId);
        }


        [Route("Close")]
        public async Task Post([FromBody] CloseLetterRequestDto request)
        {
            await LetterService.CloseLetter(request.LetterPossessionId, request.Comment);
            var letterDto = await LetterService.GetLetterDto(request.LetterPossessionId);
            var ownerFolders = request.ArchiveFolderIds.Select(x=>new OwnerFolderDto() {Id = new Guid(x) });
            await OwnerFolderService.AddToMultiFolder(letterDto, ownerFolders);
        }
         
        
    }
}