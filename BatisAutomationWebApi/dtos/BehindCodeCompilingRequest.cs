using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class BehindCodeCompilingRequest
    {
        public Guid FormId { get; set; }
        public Guid OwnerId { get; set; }
        public string ParametersValue { get; set; }
        public string TableParametersValue { get; set; }
        public string ChangedParameterName { get; set; }

    }
}