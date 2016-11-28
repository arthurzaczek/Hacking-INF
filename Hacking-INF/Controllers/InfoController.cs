using Hacking_INF.Models;
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
    public class InfoController : ApiController
    {
        private BL _bl = new BL();

        public IEnumerable<CourseViewModel> GetCourses()
        {
            return _bl.GetCourses().Select(i => new CourseViewModel(i));
        }
        public CourseViewModel GetCourse(string name)
        {
            return new CourseViewModel(_bl.GetCourses().Single(i => i.Name == name));
        }

        public IEnumerable<ExampleViewModel> GetExamples(string course)
        {
            return _bl.GetExamples(course).Select(i => new ExampleViewModel(i));
        }

        public ExampleViewModel GetExample(string course, string name)
        {
            var dir = _bl.GetExampleDir(course, name);
            var example = _bl.GetExamples(course).Single(i => i.Name == name);
            var vmdl = new ExampleViewModel(example);
            vmdl.SessionID = Guid.NewGuid();

            var angabe = Directory.GetFiles(Path.Combine(dir, "text"), "*Angabe_full.md").FirstOrDefault();
            if (angabe != null)
            {
                vmdl.Instruction = _bl.ReadTextFile(angabe);
            }

            var use_this_main = Path.Combine(dir, "src", "use_this_main.c");
            if (File.Exists(use_this_main))
            {
                vmdl.SourceCode = _bl.ReadTextFile(use_this_main);
            }
            else
            {
                vmdl.SourceCode = "int main()\n{\n}";
            }

            return vmdl;
        }
    }
}
