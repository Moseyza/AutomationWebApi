using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class DependencyTrailRequest
    {
        public Guid OwnerId { get; set; }
        public Guid LetterPossessionId { get; set; }
    }
}