using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DataTransferObjects;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Personnel")]
    public class PersonnelController : ControllerBase
    {
        [Route("All")]
        public async Task<List<PersonnelDto>> Post()
        {
            return (await PersonnelService.GetAll()).ToList();
        }
    }
}