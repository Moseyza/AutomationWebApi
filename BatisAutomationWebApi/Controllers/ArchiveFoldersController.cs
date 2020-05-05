using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    [RoutePrefix("api/ArchiveFolders")]
    [Authorize]
    public class ArchiveFoldersController : ControllerBase
    {
       
        [Route("Letters")]
        public async Task<IEnumerable<LetterDto>> Post([FromBody]ArchiveFolderDto archiveFolder)
        {
            var ownerFolder = await OwnerFolderService.GetOwnerFolder(archiveFolder.FolderId);
            var result =  await LetterService.GetAllLettersInFolder(ownerFolder);
            return result;
        }

   
    }
}