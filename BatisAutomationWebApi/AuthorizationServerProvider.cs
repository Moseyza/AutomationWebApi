using BatisServiceProvider.Services;
using DataTransferObjects;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BatisAutomationWebApi
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); 
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            try
            {
                var accountService = new AccountService();
                var branches = await accountService.Login(context.UserName, context.Password);
                var branchesId = branches.Branches.Select(x=>x.Id).Distinct().ToList();
                var user = new UserDto() { Id = branches.User.Id};
               
                identity.AddClaim(new Claim("username", context.UserName));
                identity.AddClaim(new Claim("password", context.Password));
                identity.AddClaim(new Claim("userId", branches.User.Id.ToString()));
                var propsDic = new Dictionary<string, string>();
                propsDic.Add("userId",user.Id.ToString());
                propsDic.Add("branchesCount",branchesId.Count.ToString());
                for (int i = 0; i < branchesId.Count; i++)
                {
                    propsDic.Add($"branchId{i+1}", branchesId[i].ToString());
                }


                //var letterOwnerList = letterOwners.ToList();
                //for (var i = 0; i < letterOwnerList.Count; i++)
                //{
                //    propsDic.Add($"ownerId{i+1}", letterOwnerList[i].Id.ToString());
                //    propsDic.Add($"name{i + 1}", letterOwnerList[i].Name);
                //    propsDic.Add($"nameOnly{i + 1}", letterOwnerList[i].NameOnly);
                //}
                var props = new AuthenticationProperties(propsDic);
                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
            }
            catch (Exception e)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }
            
           
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        //public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        //{
        //    return base.GrantRefreshToken(context);
        //}


    }
}