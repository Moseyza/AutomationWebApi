using System.Threading.Tasks;
using ServerWrapper.ClientSideServices.UserServices;
using DataTransferObjects;

namespace BatisServiceProvider.Services
{
    public  class AccountService :ServiceBase<ITaskedBasedUserServices>
    {
        public async Task<AccessibleBranchesInYearsDto> Login(string userName, string password)
        {
            var branches =  await Service.Authenticate(userName, password);
            return branches;
            
        }
    }
}
