using Hacking_INF.Models;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
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

        public string DataDir
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
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

        public bool IsAuthenticated
        {
            get
            {
                return System.Threading.Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? false;
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
                    user.Name = (id as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? id.Name;
                    _dal.SaveChanges();
                }

                return user;
            }

            return null;
        }

        public IPrincipal ValidateJwt(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken)) return null;

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

            var validationParameters = new TokenValidationParameters();
            validationParameters.ValidIssuer = "https://hacking-inf.technikum-wien.at";
            validationParameters.ValidAudience = "https://hacking-inf.technikum-wien.at";
            validationParameters.IssuerSigningKey = signingKey;

            try
            {
                SecurityToken validatedToken;
                return tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return null;
            }
            catch (Exception ex)
            {
                _log.Warn("Unable to validate Jwt", ex);
                return null;
            }
        }

        public string CreateJwt(UserViewModel vmdl)
        {
            var claims = new List<Claim>();

            // create required claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, vmdl.Name));
            claims.Add(new Claim(ClaimTypes.Name, vmdl.UID));
            claims.Add(new Claim(ClaimTypes.Role, string.Join(",", vmdl.Roles)));

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ExternalBearer);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = "https://hacking-inf.technikum-wien.at",
                Issuer = "https://hacking-inf.technikum-wien.at",
                Subject = identity,
                SigningCredentials = signingCredentials,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddDays(14)
            };
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(plainToken);
        }

        public void CleanupOldReportedCompilerMessages()
        {
            _dal.CleanupOldReportedCompilerMessages();
        }

        private static Dictionary<Guid, IPrincipal> _tokenList = new Dictionary<Guid, IPrincipal>();
        public Guid GetAccessToken()
        {
            if (System.Threading.Thread.CurrentPrincipal == null) throw new InvalidOperationException("Get Access Token can only be called when authenticated");

            var token = Guid.NewGuid();
            lock (_lock)
            {
                _tokenList[token] = System.Threading.Thread.CurrentPrincipal;
            }

            return token;
        }

        public IPrincipal ValidateAccessToken(Guid token)
        {
            if (token == default(Guid)) return null;

            IPrincipal user;
            lock (_lock)
            {
                if (_tokenList.TryGetValue(token, out user))
                {
                    _tokenList.Remove(token);
                    return user;
                }
            }

            return null;
        }

        private static string _secretKey = null;
        public string GetSecretKey()
        {
            lock (_lock)
            {
                if (_secretKey == null)
                {
                    var file = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data"), "SecretKey.txt");
                    if (!File.Exists(file))
                    {
                        _secretKey = Guid.NewGuid().ToString();
                        using (var sw = new StreamWriter(file))
                        {
                            sw.BaseStream.SetLength(0);
                            sw.Write(_secretKey);
                        }
                    }
                    else
                    {
                        using (var sr = new StreamReader(file))
                        {
                            _secretKey = sr.ReadToEnd().Trim();
                        }
                    }
                }

                return _secretKey;
            }
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
            if (user == null && sessionID == null) return null;
            var userUID = user?.UID;
            var qry = _dal.ExampleResults.Where(i => i.Course == course && i.Example == example);

            if (!string.IsNullOrWhiteSpace(userUID))
            {
                return qry.SingleOrDefault(i => i.User.UID == userUID);
            }
            else
            {
                return qry.SingleOrDefault(i => i.SessionID == sessionID);
            }
        }

        public IQueryable<ExampleResult> GetExampleResults()
        {
            return _dal.ExampleResults;
        }

        public ExampleResult CreateExampleResult()
        {
            return _dal.CreateExampleResult();
        }
        public ReportedCompilerMessages CreateReportedCompilerMessages()
        {
            var obj = _dal.CreateReportedCompilerMessages();
            obj.Date = DateTime.Now;
            obj.User = GetCurrentUser();
            return obj;
        }

        public IQueryable<ReportedCompilerMessages> GetReportedCompilerMessages()
        {
            return _dal.ReportedCompilerMessages;
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
                var isAuthenticated = IsAuthenticated;
                if (result == null || isTeacher)
                {
                    _log.Info("Reading & caching all examples of course " + course);
                    var path = Path.Combine(ExamplesDir, course);
                    var courseObj = GetCourses().Single(i => i.Name == course);
                    var orderList = courseObj.Categories?.Where(c => c.Examples != null).SelectMany(c => c.Examples).ToList();
                    var now = DateTime.Now;
                    result = Directory.GetDirectories(path)
                        .Select(dir =>
                        {
                            try
                            {
                                var example = ReadYAML<Example>(GetFileName(dir, "info.yaml"));
                                example.Course = course;
                                example.Name = Path.GetFileName(dir);

                                example.Order = orderList != null ? orderList.IndexOf(example.Name) : -1;
                                if (example.Order < 0) example.Order = int.MaxValue;

                                // Inheritance
                                if (example.Type == Types.NotDefined)
                                    example.Type = courseObj.Type;

                                if (string.IsNullOrWhiteSpace(example.FileName))
                                    example.FileName = courseObj.FileName;

                                if (string.IsNullOrWhiteSpace(example.Exe))
                                    example.Exe = courseObj.Exe;

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
                                if (isAuthenticated
                                 && i.OpenFrom.HasValue
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
                        .OrderBy(i => i.Order)
                        .ThenBy(i => i.Title)
                        .ToList();

                    if (!isTeacher)
                    {
                        System.Web.Hosting.HostingEnvironment.Cache.Insert("__all_examples__" + course, result, new CacheDependency(path));
                    }
                }
                return result;
            }
        }

        public IEnumerable<CompilerMessage> GetCompilerMessages()
        {
            lock (_lock)
            {
                var result = (IEnumerable<CompilerMessage>)System.Web.Hosting.HostingEnvironment.Cache.Get("__all_compiler_messages__");
                var isTeacher = IsTeacher;
                if (result == null || isTeacher)
                {
                    _log.Info("Reading & caching all compiler messages");
                    var path = Path.Combine(ExamplesDir, "compiler-messages.yaml");

                    if (File.Exists(path))
                    {
                        result = ReadYAML<IEnumerable<CompilerMessage>>(path);
                    }
                    else
                    {
                        result = new List<CompilerMessage>();
                    }

                    if (!isTeacher)
                    {
                        System.Web.Hosting.HostingEnvironment.Cache.Insert("__all_compiler_messages__", result, new CacheDependency(path));
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

        public void LogParseError(string filename, string message)
        {
            _parseErrorLog.ErrorFormat("{0}: {1}", filename, message);
        }
        public void LogParseError(string filename, Exception ex)
        {
            LogParseError(filename, ex.Message);
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