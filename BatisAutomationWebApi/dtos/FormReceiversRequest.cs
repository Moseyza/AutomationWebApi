using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class FormReceiversRequest
    {
        public Guid FormId { get; set; }
        public Guid SenderId { get; set; }
        public Guid DependentLetterId { get; set; }
    }
}