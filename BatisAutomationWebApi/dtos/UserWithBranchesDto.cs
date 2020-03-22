using System;
using System.Collections.Generic;

namespace BatisAutomationWebApi.dtos
{
    public class UserWithBranchesDto
    {
        public UserWithBranchesDto()
        {
            BranchIds = new List<Guid>();
        }
        public Guid UserId { get; set; }
        public List<Guid> BranchIds { get; set; }
    }
}