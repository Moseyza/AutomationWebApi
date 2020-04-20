using DataTransferObjects;
using ServerWrapper.ClientSideServices.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatisServiceProvider.Services
{
    public class FileService : ServiceBase<ITaskedBasedFileServices>
    {
        public async Task<FileDto> GetFile(Guid fileId)
        {
            try
            {
                return await Service.Get(fileId);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }

            
        }
    }
}
