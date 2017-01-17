using Hacking_INF.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Caching;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hacking_INF
{
    public class BL
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(BL));
        private readonly ILog _parseErrorLog = LogManager.GetLogger("ParseErrors");
        private static readonly object _lock = new object();
        private readonly IDAL _dal;

        public BL(IDAL dal)
        {
            _dal = dal;
        }

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

        public string SubmissionsDir
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Submissions");
            }
        }

        public void SaveChanges()
        {
            _dal.SaveChanges();
        }

        public bool IsTeacher
        {
            get
            {
                return System.Threading.Thread.CurrentPrincipal?.IsInRole("Teacher") ?? false;
            }
        }

        public User GetCurrentUser()
        {
            var id = System.Threading.Thread.CurrentPrincipal?.Identity;
            if (id != null && id.IsAuthenticated)
            {
                var user = _dal.Users.SingleOrDefault(i => i.UID == id.Name);
                if (user == null)
                {
                    user = _dal.CreateUser();
                    user.UID = id.Name;
                    user.Name = (id as System.Security.Claims.ClaimsIdentity)?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? id.Name;
                    _dal.SaveChanges();
                }

                return user;
            }

            return null;
        }

        public User GetUser(string uid, bool checkAccess = true)
        {
            if (!IsTeacher && checkAccess)
            {
                var id = System.Threading.Thread.CurrentPrincipal?.Identity;
                if (id?.Name != uid)
                    throw new SecurityException("You have no right to access others results");
            }
            return _dal.Users.SingleOrDefault(i => i.UID == uid);
        }

        public ExampleResult GetExampleResult(User user, Guid? sessionID, string course, string example)
        {
            var userUID = user?.UID;
            return _dal.ExampleResults.SingleOrDefault(i => (i.User.UID == userUID || i.SessionID == sessionID) && i.Course == course && i.Example == example);
        }

        public ExampleResult CreateExampleResult()
        {
            return _dal.CreateExampleResult();
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
            lock (_lock)
            {
                var result = (IEnumerable<Course>)System.Web.Hosting.HostingEnvironment.Cache.Get("__all_courses__");
                var isTeacher = IsTeacher;
                if (result == null || isTeacher)
                {
                    _log.Info("Reading & caching all courses");
                    var list = new List<Course>();
                    var fileNames = new List<string>();
                    foreach (var dir in Directory.GetDirectories(ExamplesDir))
                    {
                        try
                        {
                            string fileName = GetFileName(dir, "info.yaml");
                            fileNames.Add(fileName);
                            list.Add(ReadYAML<Course>(fileName));
                        }
                        catch (FileNotFoundException)
                        {
                            _log.WarnFormat("Directory {0} contains no info.yaml", Path.GetFileName(dir));
                        }
                    }
                    result = list
                        .Select(i =>
                        {
                            if (i.Type == Types.NotDefined)
                                i.Type = Types.Open;
                            return i;
                        })
                        .Where(i => IsTeacher || i.Type != Types.Closed)
                        .ToList();
                    if (!isTeacher)
                    {
                        System.Web.Hosting.HostingEnvironment.Cache.Insert("__all_courses__", result, new CacheDependency(fileNames.ToArray()));
                    }
                }
                return result;
            }
        }

        public IEnumerable<Example> GetExamples(string course)
        {
            lock (_lock)
            {
                var result = (IEnumerable<Example>)System.Web.Hosting.HostingEnvironment.Cache.Get("__all_examples__" + course);
                var isTeacher = IsTeacher;
                if (result == null || isTeacher)
                {
                    _log.Info("Reading & caching all examples of course " + course);
                    var path = Path.Combine(ExamplesDir, course);
                    var courseObj = GetCourses().Single(i => i.Name == course);
                    var now = DateTime.Now;
                    result = Directory.GetDirectories(path)
                        .Select(dir =>
                        {
                            try
                            {
                                var example = ReadYAML<Example>(GetFileName(dir, "info.yaml"));
                                example.Course = course;
                                example.Name = Path.GetFileName(dir);
                                if (example.Type == Types.NotDefined)
                                    example.Type = courseObj.Type;

                                return example;
                            }
                            catch
                            {
                                // Logging is done by ReadYaml
                                return null;
                            }
                        })
                        .Where(i => i != null)
                        .Where(i =>
                        {
                            if (isTeacher)
                            {
                                if (i.Type == Types.Timed)
                                {
                                    // Reflect actual state
                                    if (i.OpenFrom.HasValue
                                     && i.OpenUntil.HasValue
                                     && i.OpenFrom.Value <= now
                                     && i.OpenUntil.Value >= now)
                                    {
                                        i.Type = Types.Open;
                                    }
                                    else
                                    {
                                        i.Type = Types.Closed;
                                    }
                                }
                                return true;
                            }

                            if (i.Type == Types.Open) return true;
                            if (i.Type == Types.Closed) return false;
                            if (i.Type == Types.Timed)
                            {
                                if (i.OpenFrom.HasValue
                                 && i.OpenUntil.HasValue
                                 && i.OpenFrom.Value <= now
                                 && i.OpenUntil.Value >= now)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }

                            return false; // Fail save. Show less, maybe we're missing a exam example                  
                        })
                        .OrderBy(i => i.Title)
                        .ToList();
                    if (!isTeacher)
                    {
                        System.Web.Hosting.HostingEnvironment.Cache.Insert("__all_examples__" + course, result, new CacheDependency(path));
                    }
                }
                return result;
            }
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
            try
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
            catch (Exception ex)
            {
                LogParseError(fileName, ex);
                throw;
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
            try
            {
                if (process.HasExited) return;
            }
            catch
            {
                // Is already disposed
                return;
            }

            _log.InfoFormat("Killing process {0} ({1})", process.ProcessName, process.Id);

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

        public void LogParseError(string filename, Exception ex)
        {
            _parseErrorLog.ErrorFormat("{0}: {1}", filename, ex.Message);
        }

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        private string GetFileName(string dir, string filename)
        {
            filename = filename.ToLower();
            var lower = Path.Combine(dir, filename);
            if (File.Exists(lower)) return lower;

            var capitalized = Path.Combine(dir, FirstLetterToUpper(filename));
            if (File.Exists(capitalized)) return capitalized;

            throw new FileNotFoundException("File could not be found, neither lowercase nor capitalized", lower);
        }

        public string GetClientIp(System.Net.Http.HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
    }
}