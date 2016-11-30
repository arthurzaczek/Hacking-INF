using Hacking_INF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        public string ToolsDir
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Tools");
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
                    try
                    {
                        var example = ReadYAML<Example>(Path.Combine(dir, "info.yaml"));
                        example.Course = course;
                        example.Name = Path.GetFileName(dir);
                        return example;
                    }
                    catch (Exception ex)
                    {
                        // TODO: Log
                        return null;
                    }
                })
                .Where(i => i != null);
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

        public bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4 /* Unix */) || (p == 6 /* MacOSX */) || (p == 128 /* Mono */);
            }
        }

        public void KillProcessTree(Process process)
        {
            if (process.HasExited) return;

            var cmd = new Process();
            var sb = new StringBuilder();
            cmd.StartInfo.UseShellExecute = false;

            if (IsLinux)
            {
                cmd.StartInfo.FileName = "kill";
                cmd.StartInfo.Arguments = string.Format("-9 {0}", process.Id);
            }
            else
            {
                cmd.StartInfo.FileName = "taskkill";
                cmd.StartInfo.Arguments = string.Format("/F /PID {0} /T", process.Id);
            }

            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.OutputDataReceived += (s, e) => sb.AppendLine(e.Data);
            cmd.ErrorDataReceived += (s, e) => sb.AppendLine(e.Data);

            cmd.Start();
            cmd.BeginErrorReadLine();
            cmd.BeginOutputReadLine();
            cmd.WaitForExit();

            if (cmd.ExitCode != 0)
            {
                System.Diagnostics.Debug.WriteLine(sb.ToString());
            }
        }

        public void CleanupWorkingDir(string workingDir)
        {
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(workingDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo subdir in di.GetDirectories())
                {
                    subdir.Delete(true);
                }
            }
        }
    }
}