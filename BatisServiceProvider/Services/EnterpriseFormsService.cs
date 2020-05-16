using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Utilities;
using ServerWrapper.ClientSideServices.Services.EnterpriseForm;

namespace BatisServiceProvider.Services
{
    
    public class EnterpriseFormsService : ServiceBase<ITaskedBasedEnterpriseFormServices>
    {
        public async Task<IEnumerable<EnterpriseFormDto>> GetOwnerUsedEnterpriseForms(Guid ownerId)
        {
            var result =  await Service.GetAllThatLetterOwnerHasUsedOrSeen(ownerId);
            return result;
        }

        //public async Task<PagedResultData<EnterpriseFormDto>> GetOwnerVisibleEnterpriseForms(LetterOwnerDto owner)
        //{

        //    var result = await Service.GetAllThatLetterOwnerCanSee(owner);
        //    return result;
        //}
        //public async Task<PagedResultData<EnterpriseFormDto>> GetOwnerVisibleEnterpriseForms(LetterOwnerDto owner)
        //{

        //    var result = await Service.get(owner);
        //    return result;
        //}
    }
}
