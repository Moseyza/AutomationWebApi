using BatisServiceProvider.Services;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    public class ControllerBase : ApiController
    {
        private static AccountService _accountService;

        public static AccountService AccountService => _accountService ?? (_accountService = new AccountService());
    }
}
