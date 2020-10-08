﻿using DataTransferObjects;
using ServerWrapper.ClientSideServices.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatisServiceProvider.Services
{
    public class LetterOwnerService : ServiceBase<ITaskedBasedLetterOwnerServices>
    {
        public async Task<IEnumerable<LetterOwnerDtoWithPicture>> GetOwnersWithPicture(UserDto user,IEnumerable<BranchDto> branches)
        {
            
            return await Service.GetOwnersWithPictureThatUserInBranchHaveAccessNow(user,branches);
        }


        public async Task<IEnumerable<OwnerFolderDto>> GetArchiveFolders(Guid letterOwnerId)
        {
            var letterOwnerDto = await Service.Get(letterOwnerId);
            return await Service.GetOwnerFolders(letterOwnerDto);
            
        }

        public async Task<List<OwnerFolderDto>> GetOwnerFolders(LetterOwnerDto owner)
        {
            var folders =  await Service.GetOwnerFolders(owner);
            return folders.ToList();
        }

        public async Task<LetterOwnerDto> GetOwnerDto(Guid id)
        {
            return await Service.Get(id);
        }

       

        public async Task<IEnumerable<LetterOwnerDtoWithFaxAndEmails>> GetOwnerRecipientsWithFaxAndEmails(Guid id)
        {
            var ownerDto = await GetOwnerDto(id);
            return await Service.GetOwnersWithFaxAndEmailThatLetterOwnerCanSendLetterTo(ownerDto,new ReceiverLoadingParametersDto(),false);
        }

        public async Task<IEnumerable<LetterOwnerDto>> GetAllLetterOwners()
        {
            var result = await Service.GetAll();
            return result;
        }

        public async Task<IEnumerable<LetterOwnerDto>> GetAllCompanyLetterOwners()
        {
            try
            {
                var result = await Service.GetAllCompanyLetterOwners();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<SignDto>> GetSignFor(List<Guid> ids)
        {
            var result = await Service.GetSignFor(ids);
            return result.ToList();
        }

        public async Task<LetterOwnerIdentificationInformationDto> GetLetterOwnerIdentificationInformation(Guid ownerId)
        {
            var ownerIdentificationInfo = await Service.GetLetterOwnerIdentificationInformation(new LetterOwnerDto() {Id = ownerId});
            return ownerIdentificationInfo;
        }

        public async Task<IEnumerable<LetterOwnerDtoWithFaxAndEmails>> GetEnterpriseFormReceivers(Guid formId,Guid senderId, Guid dependentLetterId)
        {
            if (dependentLetterId == Guid.Empty)
            {
                var parameters = new ReceiverLoadingParametersDto()
                {
                    EnterpriseFormId = formId, IsEnterpriseFormKnown = true, IsSendImportant = true,
                    IsSendCopyImportant = true, IsSendDraftImportant = true
                };
                return await Service.GetOwnersWithFaxAndEmailThatLetterOwnerCanSendLetterTo(new LetterOwnerDto() {Id = senderId},parameters, true);
            }
            else
            {
                var parameters = new ReceiverLoadingParametersDto()
                {
                    EnterpriseFormId = formId, IsSendImportant = true, IsSendCopyImportant = true,
                    IsSendDraftImportant = true, IsEnterpriseFormKnown = true,
                    DependentLetters = new List<Guid>() {dependentLetterId}
                };
                return await Service.GetOwnersWithFaxAndEmailThatLetterOwnerCanSendLetterTo(new LetterOwnerDto() { Id = senderId }, parameters, true);
            }
        }
    }
}
