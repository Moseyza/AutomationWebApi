using BatisAutomationWebApi.dtos;
using BatisServiceProvider.Services;
using BatissWebOA;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

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
        [Route("ServerTime")]
        public IHttpActionResult Get()
        {
            var test = DateTime.Now.ToString( CultureInfo.InvariantCulture);
            return Ok(DateTime.Now.ToString( CultureInfo.InvariantCulture));
            
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

        [Route("RemoteLetterTrail")]
        public async Task<LetterTrailWithAttachmentsDto> Post([FromBody] RemoteLetterTrailRequestDto request)
        {
            try
            {
                var result =
                    await LetterService.GetRemoteLetterTrailForIncomingLetter(request.LetterPossessionId,
                        request.OwnerId);
                if (result == null)
                    result = await LetterService.GetRemoteLetterTrail(request.LetterPossessionId, request.OwnerId);
                return result;
            }
            catch
            {
                return null;
            }
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
        [Route("CloseFast")]
        public async Task<IHttpActionResult> Post([FromBody] CloseLetterFastRequestDto request)
        {
            try
            {
                await LetterService.CloseLetterFast(request.LetterPossessionId);
                return Ok("ok");
            }
            catch (Exception e)
            {
                return new ExceptionResult(e,null); 
            }
        }
        [Route("RejectClosedLetter")]
        public async Task<IHttpActionResult> Post([FromBody] RestoreLetterRequestDto request)
        {
            try
            {
                await LetterService.RestoreLetter(request.LetterPossessionId);
                return Ok("ok");
            }
            catch (Exception e)
            {
                return new ExceptionResult(e, null);
            }
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


        [Route("SendLetter")]
        public async Task<SendLetterAndSaveDraftResults> Post([FromBody] SendLetterDto dto)
        {
            try
            {
                var sendResults =  await LetterService.SendLetter(dto);
                IEnumerable<Guid> draftSentIds = null;
                if (dto.DraftRecievers.Any())
                {
                    //dto.ParentDraftLetterId = Guid.Empty;
                    dto.LetterId = Guid.Empty;
                    dto.Sender = null;
                    dto.Recievers = null;
                    dto.CopyRecievers = null;
                    dto.DateTime = DateTime.Now;
                    dto.Parts[0].Id = Guid.Empty;
                    if(dto.LetterRefrences == null) dto.LetterRefrences = new List<LetterReferencesToOtherLettersDto>();
                    draftSentIds = await LetterService.SaveDraft(dto);
                }

                var result = new SendLetterAndSaveDraftResults() {LetterNo =  sendResults.LetterNumber,IsAnyDraftSaved = draftSentIds?.Any()};
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Route("SaveDraft")]
        public async /*Task<IEnumerable<Guid>> */Task<SendLetterAndSaveDraftResults> Post([FromBody] SaveDraftRequest request)
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
                if (!request.IsForSender)
                {
                    request.Dto.Recievers = null;
                    request.Dto.CopyRecievers = null;
                    request.Dto.Sender = null;
                }

                var draftIds =  await  LetterService.SaveDraft(request.Dto);
                return new SendLetterAndSaveDraftResults(){IsAnyDraftSaved = draftIds.Any()};
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Route("SearchAll")]
        public async Task<IEnumerable<LetterSearchResult>> Post([FromBody] LetterSearchDto letterSearchDto)
        {
            return await LetterService.SearchAll(letterSearchDto);
        }

        [Route("Announcements")]
        public async Task<IEnumerable<AnnouncementDto>> Post([FromBody] RequestAnnouncementsDto request)
        {
            return await LetterService.GetAnnouncementsInBoard(request);
        }


        [Route("DeleteLetter")]
        public async Task Post([FromBody] LetterDto letterDto)
        {
             await LetterService.Delete(letterDto);

        }

        [Route("EmailState")]
        public async Task<MailState> Post([FromBody] EmailStateRequest request) 
        {
            var result = await LetterService.GetEmailState(request.EmailReferenceId);
            return result;
        }


        [Route("TelegramState")]
        public async Task<MailState> Post([FromBody] TelegramStateRequest request)
        {
            var result = await LetterService.GetTelegramState(request.TelegramReferenceId);
            return result;
        }


    }
}