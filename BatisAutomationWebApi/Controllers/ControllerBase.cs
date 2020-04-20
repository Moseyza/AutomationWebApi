using BatisServiceProvider.Services;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    public class ControllerBase : ApiController
    {
        private static AccountService _accountService;
        private static LetterService _letterService;
        private static BranchService _branchService;
        private static LetterOwnerService _letterOwnerService;
        private static FileService _fileService;

        public static AccountService AccountService => _accountService ?? (_accountService = new AccountService());
        public static  LetterService LetterService => _letterService ?? (_letterService = new LetterService());
        public static BranchService BranchService => _branchService ?? (_branchService = new BranchService());
        public static LetterOwnerService LetterOwnerService => _letterOwnerService ?? (_letterOwnerService = new LetterOwnerService());
        public static FileService FileService => _fileService ?? (_fileService = new FileService());
    }
}
