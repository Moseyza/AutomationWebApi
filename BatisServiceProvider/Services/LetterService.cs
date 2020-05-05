using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using ServerWrapper.ClientSideServices.UserServices;

namespace BatisServiceProvider.Services
{
    public class LetterService :ServiceBase<ITaskedBasedLetterServices>
    {
        public  async Task<LetterListerWithPaginationResult> GetLettersWithDateRange(Guid ownerId,DateTime? from,DateTime? to)
        {
            return await Service.GetRecievedLetterWithPagination(new LetterListWithPagination()
                {Owner = new LetterOwnerDto() {Id = ownerId}, From = from, To = to});
        }

        public async Task<LetterListerWithPaginationResult> GetReceivedLettersWithPagination(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetRecievedLetterWithPagination(new LetterListWithPagination() {Owner = new LetterOwnerDto() {Id = ownerId } , From = from , To = to });
        }

        public async Task<LetterListerWithPaginationResult> GetSentLettersWithPagination(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetSentLetterWithPagination(new LetterListWithPagination() { Owner = new LetterOwnerDto() { Id = ownerId }, From = from, To = to });
        }

        public async Task<LetterListerWithPaginationResult> GetDraftLettersWithPagination(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetDraftLetterWithPagination(new LetterListWithPagination() { Owner = new LetterOwnerDto() { Id = ownerId }, From = from, To = to });
        }

        public async Task<LetterTrailDto> GetLetterTrail(Guid letterPossessionId)
        {
            var letterDto = await Service.GetLetterPossession(letterPossessionId);
            return await Service.GetLetterTrail(letterDto);
        }

        public async Task<IEnumerable<LetterDto>> GetAllLettersInFolder(OwnerFolderDto ownerFolder)
        {
            return await Service.GetAllLettersInFolderReturningLetterDto(ownerFolder);
        }

        public async Task<OpenLetterResultDto> OpenLetter(Guid letterPossessionId)
        {
            
            var letterDto = await Service.GetLetterPossession(letterPossessionId);
            return await Service.OpenLetter(letterDto);
        }
    }
}
