using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.Company;

namespace BatisServiceProvider.Services
{
    public class CompanyService : ServiceBase<ITaskedBasedCompanyServices>
    {
        public async Task<IEnumerable<CompanyDto>> GetAll()
        {
            try
            {
                var pagedCompanies = await Service.GetAllPaged(-1, -1);
                return pagedCompanies.Result;
            }
            catch (Exception e)
            {
                return null;

            }
        }
    }
}
