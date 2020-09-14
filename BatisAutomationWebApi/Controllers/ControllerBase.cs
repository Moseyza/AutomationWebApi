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
        private static OwnerFolderService _ownerFolderService;
        private static EnterpriseFormsService _enterpriseFormsService;
        private static WorkflowService _workflowService;
        private static AutoCompleteDataService _autoCompleteDataService;
        private static LetterPatternService _letterPatternService;
        private static AnnouncementBoardService _announcementBoardService;
        private static CompanyService _companyService;
        private static PersonnelService _personnelService;
        


        public static AccountService AccountService => _accountService ?? (_accountService = new AccountService());
        public static  LetterService LetterService => _letterService ?? (_letterService = new LetterService());
        public static BranchService BranchService => _branchService ?? (_branchService = new BranchService());
        public static LetterOwnerService LetterOwnerService => _letterOwnerService ?? (_letterOwnerService = new LetterOwnerService());
        public static FileService FileService => _fileService ?? (_fileService = new FileService());
        public static OwnerFolderService OwnerFolderService => _ownerFolderService ?? (_ownerFolderService = new OwnerFolderService());
        public static EnterpriseFormsService EnterpriseFormsService => _enterpriseFormsService ?? (_enterpriseFormsService = new EnterpriseFormsService());
        public static WorkflowService WorkflowService => _workflowService ?? (_workflowService = new WorkflowService());
        public static AutoCompleteDataService AutoCompleteDataService => _autoCompleteDataService ?? (_autoCompleteDataService = new AutoCompleteDataService());
        public static LetterPatternService LetterPatternService =>_letterPatternService ?? (_letterPatternService = new LetterPatternService());
        public static AnnouncementBoardService AnnouncementBoardService =>_announcementBoardService ?? (_announcementBoardService = new AnnouncementBoardService());
        public static CompanyService CompanyService => _companyService ?? (_companyService = new CompanyService());
        public static PersonnelService PersonnelService => _personnelService ?? (_personnelService = new PersonnelService());
    }
}
