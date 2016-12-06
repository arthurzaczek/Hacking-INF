using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Hacking_INF.Models;
using Hacking_INF.Providers;
using Hacking_INF.Results;

namespace Hacking_INF.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        public AccountController()
        {
        }

        // POST api/Account/Login
        [Route("Login")]
        [AllowAnonymous]
        public IHttpActionResult Login(LoginViewModel vmdl)
        {
            var ldapUser = LDAPAuthenticator.Authenticate(vmdl.UID, vmdl.Password);
            if (ldapUser.IsAuthenticated)
            {
                vmdl.Roles = new[] { ldapUser.PersonalType };
                vmdl.Name = ldapUser.Fullname;

                var claims = new List<Claim>();

                // create required claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, vmdl.Name));
                claims.Add(new Claim(ClaimTypes.Name, vmdl.UID));
                claims.Add(new Claim(ClaimTypes.Role, string.Join(",", vmdl.Roles)));

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, identity);

                vmdl.Password = null; // don't send password back to client
                return Ok(vmdl);
            }
            else
            {
                return Unauthorized();
            }
        }


        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Ok();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return Request.GetOwinContext().Authentication; }
        }
    }
}
