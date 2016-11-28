using Hacking_INF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hacking_INF
{
    public class BL
    {
        public string ExamplesDir
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Examples");
            }
        }
        public string WorkingDir
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/WorkingDir");
            }
        }

        public string GetWorkingDir(Guid sessionID)
        {
            return Path.Combine(WorkingDir, sessionID.ToString());
        }

        public string GetExampleDir(string course, string name)
        {
            return Path.Combine(ExamplesDir, course, name);
        }

        public IEnumerable<Course> GetCourses()
        {
            var courses = ReadYAML<IEnumerable<Course>>(Path.Combine(ExamplesDir, "Info.yaml"));
            return courses;
        }

        public IEnumerable<Example> GetExamples(string course)
        {
            return Directory.GetDirectories(Path.Combine(ExamplesDir, course))
                .Select(dir =>
                {
                    var example = ReadYAML<Example>(Path.Combine(dir, "info.yaml"));
                    example.Course = course;
                    example.Name = Path.GetFileName(dir);
                    return example;
                });
        }

        public string ReadTextFile(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }
        public void WriteTextFile(string fileName, string content)
        {
            using (var sw = new StreamWriter(fileName))
            {
                sw.BaseStream.SetLength(0);
                sw.Write(content);
            }
        }

        public T ReadYAML<T>(string fileName)
        {
            using (var input = new StreamReader(fileName))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .IgnoreUnmatchedProperties()
                    .Build();
                return deserializer.Deserialize<T>(input);
            }
        }
    }
}