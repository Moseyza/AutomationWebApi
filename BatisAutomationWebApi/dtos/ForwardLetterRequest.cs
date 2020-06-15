using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class ForwardLetterRequest
    {
        public IEnumerable<LetterOwnerWithSendingInformationAndAttachmentsDto> MainRecipients { get; set; }
        public IEnumerable<LetterOwnerWithSendingInformationAndAttachmentsDto> CopyRecipients { get; set; }
        public Guid LetterPossessionId { get; set; }
    }
}