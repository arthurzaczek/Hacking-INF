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
            using (var input = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Info.yaml")))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();
                var courses = deserializer.Deserialize<IEnumerable<Course>>(input);

                return courses.Select(i => new CourseViewModel(i));
            }
        }
        public CourseViewModel GetCourse(string name)
        {
            using (var input = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Info.yaml")))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();
                var courses = deserializer.Deserialize<IEnumerable<Course>>(input);

                return new CourseViewModel(courses.FirstOrDefault(i => i.Name == name));
            }
        }

        public IEnumerable<ExampleViewModel> GetExamples(string course)
        {
            return Directory.GetDirectories(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/" + course))
                .Select(dir =>
                {
                    using (var input = new StreamReader(Path.Combine(dir, "info.yaml")))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(new CamelCaseNamingConvention())
                            .IgnoreUnmatchedProperties()
                            .Build();
                        var example = deserializer.Deserialize<Example>(input);
                        example.Course = course;
                        example.Name = Path.GetFileName(dir);
                        return new ExampleViewModel(example);
                    }
                });
        }
    }
}
