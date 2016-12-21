using Hacking_INF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;

namespace Hacking_INF.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private BL _bl;

        public UserController(BL bl)
        {
            _bl = bl;
        }

        public UserViewModel GetUser(string uid)
        {
            var obj = _bl.GetUser(uid);
            var vmdl = new UserViewModel(obj);
            vmdl.Results = obj
                .ExampleResults
                .OrderBy(i => i.LastAttempt)
                .Select(i => new ExampleResultViewModel(i)
                {
                    CourseTitle = _bl.GetCourses().FirstOrDefault(x => x.Name == i.Course)?.Title ?? i.Course,
                    ExampleTitle = _bl.GetExamples(i.Course).FirstOrDefault(x => x.Name == i.Example)?.Title ?? i.Course,
                });
            return vmdl;
        }
    }
}
