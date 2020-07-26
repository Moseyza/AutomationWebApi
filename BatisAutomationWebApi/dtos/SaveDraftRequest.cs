using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.dtos
{
    public class SaveDraftRequest
    {
        public SendLetterFastDto Dto { get; set; }

        public bool IsForSender {get;set;}
    }
}