using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.Workflow;

namespace BatisServiceProvider.Services
{
    public class WorkflowService : ServiceBase<ITaskedBasedWorkflowServices>
    {
        public async Task<IEnumerable<WorkflowWithEnterpriseFormDto>> GetAllWorkflowsWithItsEnterpriseForms()
        {
             return  await Service.GetAllWorkflowsWithItsEnterpriseForms();
        }
    }
}
