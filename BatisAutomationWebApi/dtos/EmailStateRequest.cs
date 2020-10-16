using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class EmailStateRequest
    {
        public Guid EmailReferenceId { get; set; }
    }

    public class TelegramStateRequest 
    {
        public Guid TelegramReferenceId { get; set; }
    }
}