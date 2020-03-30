﻿using System;
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
    }
}
