using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class ClientSideInitializeRequest
    {
        public Guid FormId { get; set; }
        public Guid OwnerId { get; set; }
        public string ParametersValue { get; set; }
        public string TableParametersValue { get; set; }
        public string ChangedParameterName { get; set; }
    }

    public class ClientSideInitialEvaluateRequest
    {
        public Guid FormId { get; set; }
        public Guid OwnerId { get; set; }
        public string ParametersValue { get; set; }
        public string TableParametersValue { get; set; }
        public string ChangedParameterName { get; set; }
    }
}