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
    [RoutePrefix("api/Workflows")]
    public class WorkFlowsController : ControllerBase
    {
        [Route("All")]
        public Task<IEnumerable<WorkflowWithEnterpriseFormDto>> Post()
        {
            return WorkflowService.GetAllWorkflowsWithItsEnterpriseForms();
        }
    }
}