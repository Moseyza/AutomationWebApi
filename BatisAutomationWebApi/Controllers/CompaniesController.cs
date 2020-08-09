using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BatisServiceProvider.Services;
using DataTransferObjects;

namespace BatisAutomationWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Companies")]
    public class CompaniesController : ControllerBase
    {
       

        [Route("All")]
        public async Task<IEnumerable<CompanyDto>>  Post()
        {
            return await CompanyService.GetAll();
        }

    }
}