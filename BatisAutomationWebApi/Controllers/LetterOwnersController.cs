using BatisAutomationWebApi.dtos;
using DataTransferObjects;
using Newtonsoft.Json.Linq;
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
    public class LetterOwnersController : ControllerBase
    {
        // GET api/<controller>
        public async Task<IEnumerable<LetterOwnerDtoWithPicture>> Post([FromBody] UserWithBranchesDto dto)
        {
            
           
            var branchDtos = await BranchService.GetBranchDtos(dto.BranchIds);
            var user = new UserDto() { Id = dto.UserId };
            return await LetterOwnerService.GetOwnersWithPicture(user, branchDtos);
        }


      



        // POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

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