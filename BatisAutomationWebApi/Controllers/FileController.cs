using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BatisAutomationWebApi.Controllers
{
    public class FileController : ControllerBase
    {
        [Authorize]
        public async Task<FileDto> Post([FromBody]FileRequestDto request)
        {
           return await FileService.GetFile(request.FileId);
        }

    
    }
}