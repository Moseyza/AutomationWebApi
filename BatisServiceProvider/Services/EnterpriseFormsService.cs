using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Utilities;
using ServerWrapper.ClientSideServices.Services.EnterpriseForm;

namespace BatisServiceProvider.Services
{
    
    public class EnterpriseFormsService : ServiceBase<ITaskedBasedEnterpriseFormServices>
    {
        public async Task<IEnumerable<EnterpriseFormDto>> GetOwnerUsedEnterpriseForms(Guid ownerId)
        {
            var result =  await Service.GetAllThatLetterOwnerHasUsedOrSeen(ownerId);
            return result;

        }

        public async Task<IEnumerable<EnterpriseFormDto>> GetLetterOwnerEnterpriseForms(LetterOwnerDto owner)
        {
            try
            {
                var result = await Service.GetAllThatLetterOwnerCanSeeAndCanStartWorkflowFromAutomationLetter(owner);
                return result.Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<EnterpriseFormDto> GetEnterpriseForm
            (Guid formId)
        {
            try
            {
                
                var result = await Service.Get(formId);
                
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        

        public  IList<Tuple<string, string>> ExecuteQuery(string query)
        {
            try
            {
                 var  result =  Service.ExecuteQuery(query);
                 return result;

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<IEnumerable<EnterpriseFormDto>> GetNextEnterpriseForms(Guid letterOwnerId, Guid letterId)
        {
            try
            {
                var forms = await Service.GetAllThatLetterOwnerCanSeeAndCanContinueWorkflowFromAutomationLetter(new LetterOwnerDto() { Id = letterOwnerId }, new LetterDto() { Id = letterId });
                return forms;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
        }

        public async Task<DraftEnterpriseFormDto> GetBasedOnDraftLetterWithValue(Guid letterId)
        {
            var result =  await Service.GetBasedOnDraftLetterWithValus(new LetterDto() {Id = letterId});
            return result;
        }



        //public async Task<PagedResultData<EnterpriseFormDto>> GetOwnerVisibleEnterpriseForms(LetterOwnerDto owner)
        //{

        //    var result = await Service.GetAllThatLetterOwnerCanSee(owner);
        //    return result;
        //}
        //public async Task<PagedResultData<EnterpriseFormDto>> GetOwnerVisibleEnterpriseForms(LetterOwnerDto owner)
        //{

        //    var result = await Service.get(owner);
        //    return result;
        //}
    }
}
