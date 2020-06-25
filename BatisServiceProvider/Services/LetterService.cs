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

        public async Task<LetterListerWithPaginationResult> GetIncomingClosedLetters(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetClosedLettersWithPagination(new LetterListWithPagination() { Owner = new LetterOwnerDto() { Id = ownerId },From = from, To = to });
        }

        public async Task<LetterListerWithPaginationResult> GetOutgoingClosedLetters(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetOutgoingClosedLettersWithPagination(new LetterListWithPagination() { Owner = new LetterOwnerDto() { Id = ownerId }, From = from, To = to });
        }

        public async Task CloseLetter(Guid letterPossessionId, string comment) 
        {
            var letterDto = await Service.GetLetterPossession(letterPossessionId);
            var closingDto = new ClosingLetterDataDto() { ClosingComment = comment };
            await Service.CloseLetter(letterDto, closingDto);
        }

        public async Task<LetterDto> GetLetterDto(Guid letterPossessionId)
        {
          return  await Service.GetLetterPossession(letterPossessionId);
        }

        public async Task<SentLetterInformationDto> ForwardLetterWithAttachments(Guid letterPossessionId,IEnumerable<LetterOwnerWithSendingInformationAndAttachmentsDto> mainRecipients,IEnumerable<LetterOwnerWithSendingInformationAndAttachmentsDto> copyRecipients)
        {
            var letterDto = await GetLetterDto(letterPossessionId);
            return await Service.ForwardLetterWithAttachments(letterDto, mainRecipients, copyRecipients);
        }

        public async Task<SentLetterInformationDto> SendLetterFast(SendLetterFastDto dto)
        {
            try
            {
                var result = await Service.SendLetterFast(dto);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Guid>> SaveDraft(SendLetterFastDto dto)
        {
            try
            {
             
                var result = await Service.SaveDraftLetter(dto);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }


    }
}
