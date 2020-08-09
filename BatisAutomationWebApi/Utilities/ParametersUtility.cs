using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BatisServiceProvider.Services;
using DataTransferObjects;

namespace BatissWebOA.Presentation.Utility
{
    public static class ParametersUtility
    {
        public static string ReplacePresianNumberCharsInDateStr(string inputStr)
        {
            var result = inputStr ?? "";
            var persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            var temp = result;
            for (int j = 0; j < persian.Length; j++)
                temp = temp.Replace(persian[j],j.ToString());
            var rgx = new Regex(@"^\d{4}/((0\d)|(1[012]))/(([012]\d)|3[01])$");
            if (rgx.IsMatch(temp))
                return temp;
            return result;
        }

        public static LetterOwnerIdentificationInformationDto _idendtificationInformation = null;
        public static async Task<string> UpdateParameters(string inputString, Guid senderId)
        {
            
            var service = new LetterOwnerService();
            if(_idendtificationInformation == null || _idendtificationInformation.LetterOwnerId != senderId.ToString())
                _idendtificationInformation = await service.GetLetterOwnerIdentificationInformation(senderId);
            var result = inputString ?? "";
            var persianTimeGetter = new PersianTimeGetter();
            result = result.Replace("%%DATE_NOW%%", persianTimeGetter.GetDateNow() ?? "");
            result = result.Replace("%%TIME_NOW%%", persianTimeGetter.GetTimeNow() ?? "");
            result = result.Replace("%%YEAR_NOW%%", persianTimeGetter.GetYearNow() ?? "");
            result = result.Replace("%%MONTH_NOW%%", persianTimeGetter.GetMonthNow() ?? "");
            result = result.Replace("%%DAY_NOW%%", persianTimeGetter.GetDayNow() ?? "");
            result = result.Replace("%%NAME%%", _idendtificationInformation.Name ?? "");
            result = result.Replace("%%LAST_NAME%%", _idendtificationInformation.LastName ?? "");
            result = result.Replace("%%ORGANIZATION_LEVEL%%", _idendtificationInformation.OrganizationalLevel ?? "");
            result = result.Replace("%%DEPARTMENT%%", _idendtificationInformation.Department ?? "");
            result = result.Replace("%%POST%%", _idendtificationInformation.Post ?? "");
            result = result.Replace("%%PERSONNEL_CODE%%", _idendtificationInformation.PersonnelCode ?? "");
            result = result.Replace("%%NATIONAL_CODE%%", _idendtificationInformation.NationalCode ?? "");
            result = result.Replace("%%COMPANY%%", _idendtificationInformation.CompanyName ?? "");
            result = result.Replace("%%COMPANY_CONTACTOR%%", _idendtificationInformation.CompanyContactorName ?? "");
            result = result.Replace("%%PERSONNEL_ID%%", _idendtificationInformation.PersonnelId ?? "");
            result = result.Replace("%%PERSONNEL_LETTEROWNER_ID%%", _idendtificationInformation.PersonnelLetterOwnerId ?? "");
            result = result.Replace("%%COMPANY_ID%%", _idendtificationInformation.CompanyId ?? "");
            result = result.Replace("%%COMPANY_LETTEROWNER_ID%%", _idendtificationInformation.CompanyLetterOwnerId ?? "");
            result = result.Replace("%%LETTEROWNER_ID%%", _idendtificationInformation.LetterOwnerId ?? "");
            return result;
        }
    }
}
