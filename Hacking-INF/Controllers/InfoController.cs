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
        public IEnumerable<CourseViewModel> GetCourses()
        {
            var courses = BL.ReadYAML<IEnumerable<Course>>(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Info.yaml"));
            return courses.Select(i => new CourseViewModel(i));
        }
        public CourseViewModel GetCourse(string name)
        {
            var courses = BL.ReadYAML<IEnumerable<Course>>(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Info.yaml"));
            return new CourseViewModel(courses.Single(i => i.Name == name));
        }

        public IEnumerable<ExampleViewModel> GetExamples(string course)
        {
            return Directory.GetDirectories(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/" + course))
                .Select(dir =>
                {
                    var example = BL.ReadYAML<Example>(Path.Combine(dir, "info.yaml"));
                    example.Course = course;
                    example.Name = Path.GetFileName(dir);
                    return new ExampleViewModel(example);
                });
        }

        public ExampleViewModel GetExample(string course, string name)
        {
            var dir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/" + course + "/" + name);
            var example = BL.ReadYAML<Example>(Path.Combine(dir, "info.yaml"));
            example.Course = course;
            example.Name = name;
            var vmdl = new ExampleViewModel(example);

            var angabe = Directory.GetFiles(Path.Combine(dir, "text"), "*Angabe_full.md").FirstOrDefault();
            if (angabe != null)
            {
                vmdl.Instruction = BL.ReadTextFile(angabe);
            }

            var use_this_main = Path.Combine(dir, "src", "use_this_main.c");
            if (File.Exists(use_this_main))
            {
                vmdl.SourceCode = BL.ReadTextFile(use_this_main);
            }
            else
            {
                vmdl.SourceCode = "int main()\n{\n}";
            }

            return vmdl;
        }
    }
}
