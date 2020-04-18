using BatisAutomationWebApi.dtos;
using BatisServiceProvider.Services;
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
    [Authorize]
    public class BranchesController : ControllerBase
    {
        public async Task<IEnumerable<BranchDto>> Post([FromBody] UserWithBranchesDto dto)
        {
            var branchDtos = await BranchService.GetBranchDtos(dto.BranchIds);
            return branchDtos;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}