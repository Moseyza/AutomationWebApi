using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.dtos
{
    public class LoadDraftEnterpriseFormRequest
    {
        public Guid PossessionId { get; set; }
        public Guid LetterId { get; set; }
        public Guid FormId { get; set; }
        public Guid OwnerId { get; set; }
    }


    public class LoadDraftEnterpriseFormResponse
    {
        public DraftEnterpriseFormDto DraftEnterpriseForm { get; set; }
        public LetterDto DraftLetter { get; set; }
        public Guid DraftLetterId { get; set; }
        public Guid SenderId { get; set; }
    }
}