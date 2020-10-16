using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class LetterTrailRequestDto
    {
        public Guid LetterPossessionId { get; set; }
    }

    public class LetterTrailWithAttachmentRequestDto
    {
        public Guid LetterPossessionId { get; set; }
        public Guid CurrentOwnerId { get; set; }
    }

    public class RemoteLetterTrailRequestDto 
    {
        public Guid LetterPossessionId { get; set; }
        public Guid OwnerId { get; set; }
    }
}