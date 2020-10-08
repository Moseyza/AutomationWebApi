using BatisAutomationWebApi.dtos;
using BatisAutomationWebApi.Utilities;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/File")]
    public class FileController : ControllerBase
    {
        
        [Route("Pdf")]
        public async Task<FileDto> Post([FromBody]FileRequestDto request)
        {
           var result = await FileService.GetFile(request.FileId);
           return result;
        }

        [Route("WordToPdf")]
        public async Task<FileDto> Post([FromBody] WordToPdfRequestDto request)
        {
            var wordFile =  await FileService.GetFile(request.WordFileId);
            var converter = new WordToPdfConverter();
            var pdfContent =  converter.GetPdfFromWordBytes(wordFile.Content,await GetBookmarks(request.LetterInfo));
            var fileDto = new FileDto() {Content = pdfContent,Extension = wordFile.Extension + ".pdf"};
            return fileDto;
        }

        private async Task<Dictionary<string, object>> GetBookmarks(SendLetterFastDto dto)
        {
            var result = new Dictionary<string,object>();
            var pc = new PersianCalendar();
            var persianDateString =$"{pc.GetYear(DateTime.Now)}/{pc.GetMonth(DateTime.Now):D2}/{pc.GetDayOfMonth(DateTime.Now):D2}";
            result.Add("LetterTitle",dto.Title);
            result.Add("LetterNo","شماره");
            result.Add("Date",persianDateString);
            result.Add("Attachment",dto.Parts.Count > 0?"دارد":"ندارد");
            var sender =  await LetterOwnerService.GetOwnerDto(dto.Sender.Id);
            var signs =  await LetterOwnerService.GetSignFor(new List<Guid>() {sender.Id});
            result.Add("SenderName",sender.NameOnly);
            result.Add("SenderPost",sender.Post);
            result.Add("Sign",signs[0].SignContent);
            if (dto.Recievers.ToList().Count > 0)
            {
                var receiver = await LetterOwnerService.GetOwnerDto(dto.Recievers.ToList()[0].Id);
                result.Add("receiverTitles", receiver.Title.Title);
            }

            var copyReceivers = "";
            foreach (var copyReceiver in dto.CopyRecievers)
            {
                var receiverDto = await LetterOwnerService.GetOwnerDto(copyReceiver.Id);
                if (copyReceivers != "") copyReceivers += "\n";
                copyReceivers += "-" + receiverDto.Title.OneLineTitle + " " + copyReceiver.SendingComment;
                
            }
            result.Add("copyReceiverTitlesWithComment", copyReceivers);
            return result;
        }


    }
}