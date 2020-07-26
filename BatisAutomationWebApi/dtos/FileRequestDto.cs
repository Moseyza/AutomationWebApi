using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.dtos
{
    public class FileRequestDto
    {
        public Guid FileId { get; set; }
    }

    public class WordToPdfRequestDto
    {
        public Guid WordFileId { get; set; }
        public SendLetterFastDto LetterInfo { get; set; }

    }
}