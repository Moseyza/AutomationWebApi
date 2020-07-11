using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.AnnouncementBoard;

namespace BatisServiceProvider.Services
{
    public  class AnnouncementBoardService: ServiceBase<ITaskedBasedAnnouncementBoardServices>
    {
        public async Task<IList<AnnouncementBoardDto>> GetAccessibleAnnouncementBoards(LetterOwnerDto dto)
        {
            return await   Service.GetAllAccessible(dto);
        }
    }
}
