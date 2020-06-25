using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using ServerWrapper.ClientSideServices.Services.LetterPattern;

namespace BatisServiceProvider.Services
{
    public class LetterPatternService: ServiceBase<ITaskedBasedLetterPatternServices>
    {
        public async Task<LetterPatternDto> GetFastPattern()
        {
            return await Service.GetFastPattern();
        }
    }
}
