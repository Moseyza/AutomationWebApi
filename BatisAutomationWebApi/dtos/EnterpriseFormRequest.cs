using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class EnterpriseFormRequest
    {
        public Guid FormId { get; set; }
        public Guid OwnerId { get; set; }
    }
}