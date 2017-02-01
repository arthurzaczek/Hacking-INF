using Hacking_INF.Models;
using Hacking_INF.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hacking_INF.Controllers
{
    [RoutePrefix("api/Info")]
    public class InfoController : ApiController
    {
        private BL _bl;
        private readonly SubmissionStoreProviderFactory _submissionStoreFactory;

        public InfoController(BL bl, SubmissionStoreProviderFactory submissionStoreFactory)
        {
            _bl = bl;
            _submissionStoreFactory = submissionStoreFactory;
        }

        [Route("GetCourses")]
        public IEnumerable<CourseViewModel> GetCourses()
        {
            return _bl.GetCourses().Select(i => new CourseViewModel(i));
        }
        [Route("GetCourse")]
        public CourseViewModel GetCourse(string name)
        {
            return new CourseViewModel(_bl.GetCourses().Single(i => i.Name == name));
        }
        [Route("GetCategories")]
        public IEnumerable<CategoryViewModel> GetCategories(string course)
        {
            return _bl.GetCourses().Single(i => i.Name == course).Categories?.Select(i => new CategoryViewModel(i));
        }

        [Route("GetExamples")]
        public IEnumerable<ExampleViewModel> GetExamples(string course)
        {
            return _bl.GetExamples(course).Select(i => new ExampleViewModel(i));
        }

        [Route("GetExample")]
        public ExampleViewModel GetExample(string course, string name)
        {
            var dir = _bl.GetExampleDir(course, name);
            var example = _bl.GetExamples(course).Single(i => i.Name == name);
            var user = _bl.GetCurrentUser();
            var pastResult = _bl.GetExampleResult(user, null, course, name);

            var vmdl = new ExampleViewModel(example);
            vmdl.SessionID = Guid.NewGuid();
            vmdl.StartTime = DateTime.Now;

            var angabe = Directory.GetFiles(Path.Combine(dir, "text"), "*Angabe_full.md").FirstOrDefault();
            if (angabe != null)
            {
                vmdl.Instruction = _bl.ReadTextFile(angabe);
            }

            var use_this_main = Directory.GetFiles(Path.Combine(dir, "src"), "use_this_main.*").FirstOrDefault();
            if (use_this_main != null)
            {
                vmdl.UseThisMain = _bl.ReadTextFile(use_this_main);
            }
            else
            {
                vmdl.UseThisMain = "";
            }

            if (pastResult != null && user != null)
            {
                var store = _submissionStoreFactory(course, name, user.UID);
                var main = store.GetItems().FirstOrDefault(i => i.Name == example.FileName);
                if (main != null)
                {
                    using (var sr = new StreamReader(main.GetStream()))
                    {
                        vmdl.SourceCode = sr.ReadToEnd();
                    }
                }
            }

            if(string.IsNullOrWhiteSpace(vmdl.SourceCode))
            {
                vmdl.SourceCode = vmdl.UseThisMain;
            }

            return vmdl;
        }
    }
}
