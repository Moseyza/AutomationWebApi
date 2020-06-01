using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class CloseLetterRequestDto
    {
        public Guid LetterPossessionId { get; set; }
        public string Comment { get; set; }
        public List<string> ArchiveFolderIds { get; set; }
        public List<string> AttachmentNames { get; set; }
        public List<byte[]> AttachmentContents { get; set; }
    }
}