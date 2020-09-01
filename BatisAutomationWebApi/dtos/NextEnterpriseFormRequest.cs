using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.dtos
{
    public class NextEnterpriseFormRequest
    {
        public Guid FormId { get; set; }
        public Guid LetterId { get; set; }
        public Guid PossessionId { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class NextEnterpriseFormResponse
    {
        public EnterpriseFormDto EnterpriseForm { get; set; }
        public MultipleEnterpriseFormValuesDto MultipleValues { get; set; }
        public Guid SenderId { get; set; }
        public Guid DependentLetterId { get; set; }
        public Guid AnnouncementBoardIdToSend { get; set; }

    }
}