using Hacking_INF.Models;
using Hacking_INF.Providers;
using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Hacking_INF.Controllers
{
    [RoutePrefix("api/Admin")]
    [Authorize(Roles = "Teacher")]
    public class AdminController : ApiController
    {
        private readonly BL _bl;
        private readonly SubmissionCollectorFactory _submissionCollectorFactory;
        private readonly ILog _log = LogManager.GetLogger(typeof(AdminController));

        public AdminController(BL bl, SubmissionCollectorFactory submissionCollectorFactory)
        {
            _bl = bl;
            _submissionCollectorFactory = submissionCollectorFactory;
        }

        [Route("Download")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Download(string course, string example, Guid token)
        {
            var user = _bl.ValidateAccessToken(token);
            if (user == null || !user.IsInRole("Teacher")) return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (string.IsNullOrWhiteSpace(example))
            {
                example = ".*";
            }

            var stores = _submissionCollectorFactory(course, example);

            string fileName = string.Format("Submissions-{0}.zip", course);
            var zip = new ZipFile();
            var toDispose = new List<IDisposable>();

            foreach (var store in stores)
            {
                try
                {
                    foreach (var item in store.GetItems(includeHidenFiles: true))
                    {
                        var s = item.GetStream();
                        zip.AddEntry(store.UID + "\\" + item.FilePath, s);
                        toDispose.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    _log.Warn(string.Format("Unable to download from repo {0}, continue", store.UID), ex);
                }
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            // Not very memory friendly
            var stream = new MemoryStream();
            zip.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            zip.Dispose();
            foreach (var d in toDispose)
            {
                d.Dispose();
            }

            return result;
        }

        [Route("GetStats")]
        [HttpGet]
        public ExampleStatViewModel[] GetStats()
        {
            var results = _bl.GetExampleResults()
                .GroupBy(i => i.Course + "-" + i.Example)
                .ToList() // exec query
                .Select(i => new
                {
                    Course = i.FirstOrDefault().Course,
                    Example = i.FirstOrDefault().Example,
                    FirstAttempt = i.Min(p => p.FirstAttempt),
                    LastAttempt = i.Max(p => p.LastAttempt),
                    NumOfAttempts = i.Count(),

                    StdDevTime = i.Select(p => (double)p.Time).StdDev(),
                    AvgTime = i.Where(p => p.Time.HasValue && p.Time > 0)
                               .Select(p => p.Time)
                               .OrderBy(t => t)
                               .Take((int)(i.Count(p => p.Time.HasValue && p.Time > 0) * 0.9))
                               .Average() ?? 0,
                    MedTime = i.Where(p => p.Time.HasValue && p.Time > 0).Median(p => p.Time) ?? 0,

                    AvgNumOfTestRuns = i.Average(p => p.NumOfTestRuns),
                    AvgNumOfSucceeded = i.Average(p => p.NumOfSucceeded),
                    NumOfTests = i.Max(p => p.NumOfTests),
                }).ToList();

            return results
                .OrderBy(i => i.Course)
                .ThenBy(i => i.Example)
                .Select(i => new ExampleStatViewModel()
                {
                    Course = i.Course,
                    CourseTitle = i.Course,
                    Example = i.Example,
                    ExampleTitle = i.Example,

                    FirstAttempt = i.FirstAttempt,
                    LastAttempt = i.LastAttempt,
                    NumOfAttempts = i.NumOfAttempts,

                    AvgTime = TimeSpan.FromSeconds(i.AvgTime).ToString(@"hh\:mm\:ss"),
                    MedTime = TimeSpan.FromSeconds(i.MedTime).ToString(@"hh\:mm\:ss"),
                    StdDevTime = i.StdDevTime,

                    AvgNumOfTestRuns = i.AvgNumOfTestRuns,
                    AvgNumOfSucceeded = i.AvgNumOfSucceeded,
                    NumOfTests = i.NumOfTests,
                }).ToArray();
        }

        [Route("GetStatsStudents")]
        [HttpGet]
        public StudentStatViewModel[] GetStatsStudents(string course, int year)
        {
            return _bl.GetExampleResults()
                .Where(i => i.Course == course)
                .Where(i => i.User != null)
                .Where(i => year <= 0 || i.FirstAttempt.Year == year || i.LastAttempt.Year == year)
                .GroupBy(i => i.User)
                .OrderBy(i => i.Key.UID)
                .ToList() // exec query
                .Select(s => new StudentStatViewModel()
                {
                    User = s.Key.Name,
                    UID = s.Key.UID,

                    FirstAttempt = s.Min(i => i.FirstAttempt),
                    LastAttempt = s.Max(i => i.LastAttempt),

                    Time = TimeSpan.FromSeconds(s.Sum(i => i.Time ?? 0)).ToString(@"hh\:mm\:ss"),
                    NumOfTestRuns = s.Sum(i => i.NumOfTestRuns),
                    NumOfSucceeded = s.Sum(i => i.NumOfSucceeded),
                    NumOfTests = s.Sum(i => i.NumOfTests),

                    Details = s.OrderBy(i => i.Example).Select(i => new StudentStatDetailViewModel()
                    {
                        Course = i.Course,
                        CourseTitle = i.Course,
                        Example = i.Example,
                        ExampleTitle = i.Example,

                        FirstAttempt = i.FirstAttempt,
                        LastAttempt = i.LastAttempt,

                        Time = TimeSpan.FromSeconds(i.Time ?? 0).ToString(@"hh\:mm\:ss"),

                        NumOfTestRuns = i.NumOfTestRuns,
                        NumOfSucceeded = i.NumOfSucceeded,
                        NumOfTests = i.NumOfTests,
                    }).ToArray()
                })
                .ToArray();
        }

        [Route("GetStatsStudentsCSV")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetStatsStudentsCSV(string course, int year, Guid token)
        {
            var user = _bl.ValidateAccessToken(token);
            if (user == null || !user.IsInRole("Teacher")) return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            var qry = _bl.GetExampleResults()
                .Where(i => i.Course == course)
                .Where(i => i.User != null)
                .Where(i => year <= 0 || i.FirstAttempt.Year == year || i.LastAttempt.Year == year)
                .GroupBy(i => i.User)
                .OrderBy(i => i.Key.UID)
                .ToList() // exec query
                .Select(s => new
                {
                    User = s.Key.Name,
                    UID = s.Key.UID,

                    FirstAttempt = s.Min(i => i.FirstAttempt),
                    LastAttempt = s.Max(i => i.LastAttempt),

                    Time = TimeSpan.FromSeconds(s.Sum(i => i.Time ?? 0)).ToString(@"hh\:mm\:ss"),
                    NumOfTestRuns = s.Sum(i => i.NumOfTestRuns),
                    NumOfSucceeded = s.Sum(i => i.NumOfSucceeded),
                    NumOfTests = s.Sum(i => i.NumOfTests),
                });

            var sb = new StringBuilder();
            sb.AppendLine("User;UID;FirstAttempt;LastAttempt;Time;NumOfTestRuns;NumOfSucceeded;NumOfTests");
            foreach (var s in qry)
            {
                sb.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7}", 
                    s.User, 
                    s.UID,
                    s.FirstAttempt,
                    s.LastAttempt,
                    s.Time,
                    s.NumOfTestRuns,
                    s.NumOfSucceeded,
                    s.NumOfTests));
            }

            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0}-Summary.csv", course)
            };

            return result;
        }

        [Route("GetStatsStudentDetailsCSV")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetStatsStudentDetailsCSV(string course, int year, Guid token)
        {
            var user = _bl.ValidateAccessToken(token);
            if (user == null || !user.IsInRole("Teacher")) return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            var qry = _bl.GetExampleResults()
                .Where(i => i.Course == course)
                .Where(i => i.User != null)
                .Where(i => year <= 0 || i.FirstAttempt.Year == year || i.LastAttempt.Year == year)
                .OrderBy(i => i.User.UID)
                .ThenBy(i => i.Example)
                .ToList() // exec query
                .Select(i => new
                {
                    User = i.User.Name,
                    UID = i.User.UID,

                    Example = i.Example,
                    ExampleTitle = i.Example,

                    FirstAttempt = i.FirstAttempt,
                    LastAttempt = i.LastAttempt,

                    Time = TimeSpan.FromSeconds(i.Time ?? 0).ToString(@"hh\:mm\:ss"),

                    NumOfTestRuns = i.NumOfTestRuns,
                    NumOfSucceeded = i.NumOfSucceeded,
                    NumOfTests = i.NumOfTests,
                });

            var sb = new StringBuilder();
            sb.AppendLine("User;UID;Example;FirstAttempt;LastAttempt;Time;NumOfTestRuns;NumOfSucceeded;NumOfTests");
            foreach (var s in qry)
            {
                sb.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                    s.User,
                    s.UID,
                    s.Example,
                    s.FirstAttempt,
                    s.LastAttempt,
                    s.Time,
                    s.NumOfTestRuns,
                    s.NumOfSucceeded,
                    s.NumOfTests));
            }

            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0}-Details.csv", course)
            };

            return result;
        }

        [Route("GetReportedCompilerMessages")]
        [HttpGet]
        public ReportedCompilerMessageViewModel[] GetReportedCompilerMessages()
        {
            return _bl.GetReportedCompilerMessages()
                .OrderBy(i => i.Course)
                .ThenBy(i => i.Example)
                .ThenByDescending(i => i.Date)
                .ToList()
                .Select(i => new ReportedCompilerMessageViewModel(i)
                {
                    CourseTitle = i.Course,
                    ExampleTitle = i.Example,
                })
                .ToArray();
        }

        [Route("GetLogfile")]
        [HttpGet]
        public LogLineModelViewModel[] GetLogfile(string type)
        {
            string logfile = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Logs");

            switch (type)
            {
                case "log":
                    logfile = Path.Combine(logfile, "Web.txt");
                    break;
                case "parser":
                    logfile = Path.Combine(logfile, "ParseErrors.txt");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", "unknown logfile " + type);
            }

            List<LogLineModelViewModel> mdl = new List<LogLineModelViewModel>();
            if (System.IO.File.Exists(logfile))
            {
                using (var sr = new StreamReader(logfile))
                {
                    while (!sr.EndOfStream)
                    {
                        mdl.Add(new LogLineModelViewModel() { Message = sr.ReadLine() });
                    }
                }
            }
            return mdl.ToArray();
        }

        [Route("UpdateExamples")]
        [HttpPost]
        public string UpdateExamples()
        {
            _bl.UpdateExamples();
            return "Updating examples was sucessful!";
        }

        [Route("TriggerUpdateExamples")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage TriggerUpdateExamples(string token)
        {
            _log.Info("Trigger update examples");

            var settings = _bl.ReadYAML<ExamplesRepo>(Path.Combine(_bl.SettingsDir, "ExamplesRepo.yaml"));
            if (settings.UpdateToken != token)
            {
                _log.Error("Invalid token");
                var error = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                error.Content = new StringContent("Invalid token", Encoding.UTF8, "text/plain");
                return error;
            }

            _bl.UpdateExamples();

            var ok = Request.CreateResponse(HttpStatusCode.OK);
            ok.Content = new StringContent("Success!", Encoding.UTF8, "text/plain");
            return ok;
        }
    }
}
