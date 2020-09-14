using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.dtos
{
    public class SendingFormResults
    {
        public bool HasError { get; set; }
        public SentLetterInformationDto SentLetterInformation { get; set; }
        public string ErrorMessage { get; set; }
    }
}