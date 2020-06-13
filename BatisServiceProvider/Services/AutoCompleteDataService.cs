using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.AutoCompleteData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatisServiceProvider.Services
{
    public class AutoCompleteDataService : ServiceBase<ITaskedBasedAutoCompleteDataServices>
    {
        public async Task<IEnumerable<AutoCompleteDataDto>> GetCloseAutoCompleteData()
        {
            return await Service.GetAllCloseAutoCompleteData();
            
        }

        public async Task<IEnumerable<AutoCompleteDataDto>> GetAllForwardingAutoCompleteData()
        {
            return await Service.GetAllForwardingAutoCompleteData();
        }
    }
}
