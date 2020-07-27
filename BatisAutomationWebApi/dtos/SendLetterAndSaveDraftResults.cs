using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatisAutomationWebApi.dtos
{
    public class SendLetterAndSaveDraftResults
    {
        public string LetterNo { get; set; }
        public bool? IsAnyDraftSaved { get; set; }
    }
}