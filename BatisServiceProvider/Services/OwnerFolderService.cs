using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.OwnerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatisServiceProvider.Services
{
    public class OwnerFolderService : ServiceBase<ITaskedBasedOwnerFolderServices>
    {
        public async Task<OwnerFolderDto> GetOwnerFolder(Guid folderId)
        {
            return await  Service.Get(folderId);
           
        }

        public async Task AddToMultiFolder(LetterDto letterdto, IEnumerable<OwnerFolderDto> ownerFolders)
        {
            await Service.AddToMutiFolder(letterdto,ownerFolders);
        }
    }
}
