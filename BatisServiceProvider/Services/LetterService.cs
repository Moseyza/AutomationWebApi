using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using ServerWrapper.ClientSideServices.UserServices;

namespace BatisServiceProvider.Services
{
    public class LetterService : ServiceBase<ITaskedBasedLetterServices>
    {
        public async Task<LetterListerWithPaginationResult> GetLettersWithDateRange(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetRecievedLetterWithPagination(new LetterListWithPagination()
            { Owner = new LetterOwnerDto() { Id = ownerId }, From = from, To = to });
        }

        public async Task<LetterListerWithPaginationResult> GetReceivedLettersWithPagination(Guid ownerId, DateTime? from, DateTime? to)
        {
            return await Service.GetRecievedLetterWithPagination(new LetterListWithPagination() { Owner = new LetterOwnerDto() { Id = ownerId }, From = from, To = to });
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

        public async Task<LetterTrailWithAttachmentsDto> GetRemoteLetterTrail(Guid letterPossessionId, Guid letterOwnerId) 
        {
            var letterDto = new LetterDto() { LetterPossessionId = letterPossessionId };
            var result = await Service.GetRemoteLetterTrailForIncommingLetter(letterDto, letterOwnerId);
            return result; 
        }

        public async Task<LetterTrailWithAttachmentsDto> GetLetterTrailWithAttachment(Guid letterPossessionId, Guid currentOwnerId)
        {
            var letterDto = await Service.GetLetterPossession(letterPossessionId);
            var result = await Service.GetLetterTrailWithAttachmentWithPermissions(letterDto, currentOwnerId);
            return result;
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
            return await Service.GetClosedLettersWithPagination(new LetterListWithPagination() { Owner = new LetterOwnerDto() { Id = ownerId }, From = from, To = to });
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

        public async Task CloseLetterFast(Guid letterPossessionId)
        {
            var letterDto = await Service.GetLetterPossession(letterPossessionId);
            await Service.CloseLetter(letterDto, new ClosingLetterDataDto());

        }

        public async Task RestoreLetter(Guid letterPossessionId)
        {
            var letterDto = await Service.GetLetterPossession(letterPossessionId);
            //await Service.RestoreLetter(letterDto);
            await Service.RejectLetterConfirmation(letterDto);
        }

        public async Task<DraftLetterDto> GetDraftLetter(Guid possessionId)
        {
            try
            {
                var result = await Service.GetSingleDraft(possessionId);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<LetterListerWithPaginationResult> GetAllSavedDraftsWithPagination(Guid ownerId,DateTime? from, DateTime? to)
        {
            var result = await Service.GetDraftLetterWithPagination(new LetterListWithPagination()
                {From = from, To = to, Owner = new LetterOwnerDto() {Id = ownerId}});
            return result;
        }

        public async Task<IDictionary<string,IDictionary<string,string>>> ReloadFormData(Guid formId,Dictionary<string,string> accountInfo,IDictionary<string,IDictionary<string,string>> request)
        {
            return await Service.ReloadFormData(formId, accountInfo, request);
        }

        public async Task<LetterDto> GetLetterDto(Guid letterPossessionId)
        {
            return await Service.GetLetterPossession(letterPossessionId);
        }

        public async Task<SentLetterInformationDto> ForwardLetterWithAttachments(Guid letterPossessionId, IEnumerable<LetterOwnerWithSendingInformationAndAttachmentsDto> mainRecipients, IEnumerable<LetterOwnerWithSendingInformationAndAttachmentsDto> copyRecipients)
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

        public async Task<SentLetterInformationDto> SendLetter(SendLetterDto dto)
        {
            try
            {
                var result = await Service.SendLetter(dto);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }



        public async Task<IEnumerable<Guid>> SaveDraft(SendLetterDto dto)
        {
            //try
            //{

            var result = await Service.SaveDraftLetter(dto);
            return result;
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}
        }

        public async Task<IEnumerable<LetterSearchResult>> SearchAll(LetterSearchDto searchDto)
        {
            try
            {
                var result = await Service.SearchAll(searchDto);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<IEnumerable<AnnouncementDto>> GetAnnouncementsInBoard(RequestAnnouncementsDto request)
        {
            try
            {
                var result = await Service.GetAnnouncementsInBoard(request);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task Delete(LetterDto letterDto)
        {
            try
            {
                await Service.LogicalDeleteLetter(letterDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return null;
            }

        }

        public EnterpriseFormVaildValuesForTableColumnDto GetFormValidValues(Guid formId)
        {
            try
            {

                return Service.GetValidValuesForEnterpriseFormTableColumns(formId);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Guid> GetAnnouncementBoardIdForLetter(Guid letterId)
        {
            try
            {
                return await Service.GetAnnouncementBoardIdForLetter(letterId);
            }
            catch (Exception e)
            {
                return Guid.Empty;
            }

        }

        public async Task<MultipleEnterpriseFormValuesDto> GetMultipleEnterpriseFormValues(Guid letterId)
        {
            try
            {
                var result = await Service.GetMultipleEnterpriseFormValuesDto(new RequestMultipleEnterpriseFormValuesDto() { LetterIds = new List<Guid>() { letterId } });
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Dictionary<string, string>> ReloadTableData(Guid id, Dictionary<string, string> accountInfo,
            IDictionary<string, IDictionary<string, string>> evaluationResultRequestForReloadingTableValues)
        {
            try
            {
                var result = await Service.ReloadTableData(id, accountInfo, evaluationResultRequestForReloadingTableValues);
                return result.ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public async Task<string> ServerSideInitialize(Guid enterpriseFormId, Dictionary<string, string> senderInfo,
            string parameterName)
        {
            try
            {
                var result = await Service.ServerSideInitialize(enterpriseFormId, senderInfo, parameterName);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public async Task<SentLetterInformationDto> SendEnterpriseForm(SendEnterpriseFormDto enterpriseFormDto)
        {
            var result = await Service.SendEnterpriseForm(enterpriseFormDto);
            return result;
        }

        public async Task<IEnumerable<Guid>> SaveEnterpriseFormDraft(SendEnterpriseFormDto sendEnterpriseFormDto)
        {
            var result = await Service.SaveDraftEnterpriseForm(sendEnterpriseFormDto);
            return result;
        }

        public async Task<EnterpriseFormValuesDto> GetEnterpriseFormValues(Guid letterId, Guid possessionId)
        {
            try
            {
                var result = await Service.GetEnterpriseFormValuesDto(new RequestEnterpriseFormValuesDto() { LetterId = letterId, LetterPossessionId = possessionId });
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<MailState> GetEmailState(Guid id) 
        {
            var state = await Service.GetEmailStatus(id);
            return state;

        }

        public async Task<MailState> GetTelegramState(Guid id)
        {
            var state = await Service.GetMessagingAppMessageState(id);
            return state;

        }

        public async Task<DependencyTrailDto> GetDependencyTrail(Guid ownerId,Guid letterPossessionId) 
        {
            var result = await Service.GetDependencyTrail(new DependencyTrailRequest() { CurrentLetterOwnerId = ownerId, LetterPossessionId = letterPossessionId });
            return result;
        }

    }
}
