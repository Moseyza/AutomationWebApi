using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatisServiceProvider.Services
{
    public class BranchService:ServiceBase<ITaskedBasedBranchServices>
    {
        public async Task<IList<BranchDto>> GetBranchDtos(IList<Guid> branchIds)
        {
            var result = new List<BranchDto>();
            foreach (var id in branchIds)
            {
                result.Add( await Service.Get(id));
            }
            return result;

        }
    }
}
