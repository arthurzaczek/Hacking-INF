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
using System.Text;

namespace Hacking_INF.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private BL _bl;

        public AccountController(BL bl)
        {
            _bl = bl;
        }

        // POST api/Account/Login
        [Route("Login")]
        [AllowAnonymous]
        public IHttpActionResult Login(UserViewModel vmdl)
        {
            var ldapUser = LDAPAuthenticator.Authenticate(vmdl.UID, vmdl.Password);
            if (ldapUser.IsAuthenticated)
            {
                vmdl.Roles = new[] { ldapUser.PersonalType };
                vmdl.Name = ldapUser.Fullname;
                vmdl.Password = null; // don't send password back to client
                vmdl.Jwt = _bl.CreateJwt(vmdl);

                return Ok(vmdl);
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET api/Account/WhoAmI
        [Route("WhoAmI")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult WhoAmI()
        {
            UserViewModel vmdl = null;
            var user = _bl.GetCurrentUser();
            if(user != null)
            {
                var p = System.Threading.Thread.CurrentPrincipal;
                vmdl = new UserViewModel();
                vmdl.UID = user.UID;
                vmdl.Name = user.Name;
                vmdl.Roles = p.IsInRole("Teacher") ? new[] { "Teacher" } : new string[] { };
                vmdl.Jwt = _bl.CreateJwt(vmdl); // Refresh token
            }
            return Ok(vmdl);
        }

        // GET api/Account/GetToken
        [Route("GetToken")]
        [HttpGet]
        public IHttpActionResult GetToken()
        {
            var user = _bl.GetCurrentUser();
            if (user != null)
            {
                return Ok(_bl.GetAccessToken());
            }
            return Unauthorized();
        }
    }
}
