using BatisServiceProvider.Services;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    public class ControllerBase : ApiController
    {
        private static AccountService _accountService;
        private static LetterService _letterService;

        public static AccountService AccountService => _accountService ?? (_accountService = new AccountService());
        public static  LetterService LetterService => _letterService ?? (_letterService = new LetterService());
    }
}
