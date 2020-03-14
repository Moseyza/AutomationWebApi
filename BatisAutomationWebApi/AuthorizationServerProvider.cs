using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using BatisServiceProvider.Services;
using DataTransferObjects;
using Microsoft.Owin.Security;

namespace BatisAutomationWebApi
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); // 
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            try
            {
                var accountService = new AccountService();
                var branches = await accountService.Login(context.UserName, context.Password);

                identity.AddClaim(new Claim("username", context.UserName));
                identity.AddClaim(new Claim("password", context.Password));
                identity.AddClaim(new Claim("userId", branches.User.Id.ToString()));
                //var props = new AuthenticationProperties(new Dictionary<string, string>
                //{
                //    {
                //        "surname", "Smith"
                //    },
                //    {
                //        "age", "20"
                //    },
                //    {
                //        "gender", "Male"
                //    }
                //});
                //var ticket = new AuthenticationTicket(identity, props);
                context.Validated(identity);
            }
            catch (Exception e)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }
            
            
            
            
            //if (context.UserName == "admin" && context.Password == "admin")
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            //    identity.AddClaim(new Claim("username", "admin"));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "Sourav Mondal"));
            //    context.Validated(identity);
            //}
            //else if (context.UserName == "user" && context.Password == "user")
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            //    identity.AddClaim(new Claim("username", "user"));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "Suresh Sha"));
            //    context.Validated(identity);
            //}
            //else
            //{
                
            //    context.SetError("invalid_grant", "Provided username and password is incorrect");
            //    return;
            //}
        }

    }
}