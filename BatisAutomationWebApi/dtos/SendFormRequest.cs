using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.dtos
{
    public class SendFormRequest
    {
        public Guid FormId { get; set; }
        public Guid SenderId { get; set; }
        public string Bookmarks { get; set; }
        public string TableBookmarks { get; set; }
        public IEnumerable<LetterOwnerDtoForSendingFaxAndEmailAndSms> DraftReceivers { set; get; }
        public IEnumerable<LetterOwnerDtoForSendingFaxAndEmailAndSms> Receivers { set; get; }
        public IEnumerable<LetterOwnerDtoForSendingFaxAndEmailAndSms> CopyReceivers { set; get; }
        public List<AttachedFile> AttachedFiles { get; set; }
    }

    public class AttachedFile
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
    }
}