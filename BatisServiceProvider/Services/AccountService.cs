using System.Threading.Tasks;
using ServerWrapper.ClientSideServices.UserServices;

namespace BatisServiceProvider.Services
{
    public  class AccountService :ServiceBase<ITaskedBasedUserServices>
    {
        public async Task Login(string userName, string password)
        {
            var loginResult =  await Service.Authenticate(userName, password);
            
        }
    }
}
