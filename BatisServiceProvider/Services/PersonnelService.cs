using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.Personnel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatisServiceProvider.Services
{
    public class PersonnelService : ServiceBase<ITaskedBasedPersonnelServices>
    {
        public async Task<IEnumerable<PersonnelDto>> GetAll()
        {
            try
            {
                var pagedPersonnel = await Service.GetAllPaged(-1, -1);
                return pagedPersonnel.Result;
            }
            catch (Exception e)
            {
                return null;

            }
        }
    }
}
