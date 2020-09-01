using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class NextFormsListRequest
    {
        public Guid LetterId { get; set; }
        public Guid LetterOwnerId { get; set; }
        public Guid PossessionId { get; set; }
    }
}