using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using BatisServiceProvider.Services;
using BatissWebOA;

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
            var result =  await LetterService.GetLetterTrail(request.LetterPossessionId);
            return result;
        }

        [Route("LetterTrailWithAttachment")]
        public async Task<LetterTrailWithAttachmentsDto> Post([FromBody]LetterTrailWithAttachmentRequestDto request)
        {
            var result = await LetterService.GetLetterTrailWithAttachment(request.LetterPossessionId,request.CurrentOwnerId);
            return result;
        }



        [Route("Close")]
        public async Task Post([FromBody] CloseLetterRequestDto request)
        {
            await LetterService.CloseLetter(request.LetterPossessionId, request.Comment);
            var letterDto = await LetterService.GetLetterDto(request.LetterPossessionId);
            var ownerFolders = request.ArchiveFolderIds.Select(x=>new OwnerFolderDto() {Id = new Guid(x) });
            await OwnerFolderService.AddToMultiFolder(letterDto, ownerFolders);
        }
        [Route("Forward")]
        public async Task<SentLetterInformationDto> Post([FromBody] ForwardLetterRequest request)
        {
            try {
                var result = await LetterService.ForwardLetterWithAttachments(request.LetterPossessionId, request.MainRecipients, request.CopyRecipients);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        [Route("SendLetterFast")]
        public async Task<SentLetterInformationDto> Post([FromBody]SendLetterFastDto dto)
        {
            try
            {
                return await LetterService.SendLetterFast(dto);
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        [Route("SaveDraft")]
        public async Task<IEnumerable<Guid>> Post([FromBody] SaveDraftRequest request)
        {
            try
            {
                var pattern = await LetterPatternService.GetFastPattern();
                var patternFile = await FileService.GetFile(pattern.File.Id);
                var wordChanger = new WordChanger();
                var bookmarks = new Dictionary<string, object>() { { "Content",request.Dto.StringContent } };
                var mainPart = wordChanger.SetBookmarks(patternFile.Content, bookmarks);
                patternFile.Content = mainPart;
                patternFile.Extension = @"اصل نامه.docx";
                request.Dto.Parts.Insert(0,new PartsDto() { Id = patternFile.Id,File = patternFile});
                return await  LetterService.SaveDraft(request.Dto);
            }
            catch (Exception e)
            {
                return null;
            }
        }


    }
}