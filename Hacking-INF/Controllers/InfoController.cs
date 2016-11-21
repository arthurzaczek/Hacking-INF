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
    }
}
