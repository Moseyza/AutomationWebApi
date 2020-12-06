using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class UpdateParameterRequest
    {
        public Guid OwnerId { get; set; }
        public string Value { get; set; }
    }
}