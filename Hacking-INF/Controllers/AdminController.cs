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
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };

            zip.Dispose();
            foreach (var d in toDispose)
            {
                d.Dispose();
            }

            return result;
        }

        [Route("GetStats")]
        [HttpGet]
        public ExampleResultViewModel[] GetStats()
        {
            var results = _bl.GetExampleResults().GroupBy(i => i.Course + "-" + i.Example).Select(i => new
            {
                Course = i.FirstOrDefault().Course,
                Example = i.FirstOrDefault().Example,
                FirstAttempt = i.Min(p => p.FirstAttempt),
                LastAttempt = i.Max(p => p.LastAttempt),
                NumOfAttempts = i.Count(),

                Time = i.Average(p => p.Time),

                NumOfTestRuns = i.Average(p => p.NumOfTestRuns),
                NumOfSucceeded = i.Average(p => p.NumOfSucceeded),
                NumOfTests = i.Max(p => p.NumOfTests),
            }).ToList();

            return results
                .OrderBy(i => i.Course)
                .ThenBy(i => i.Example)
                .Select(i => new ExampleResultViewModel()
                {
                    Course = i.Course,
                    CourseTitle = i.Course,
                    Example = i.Example,
                    ExampleTitle = i.Example,

                    FirstAttempt = i.FirstAttempt,
                    LastAttempt = i.LastAttempt,
                    NumOfAttempts = i.NumOfAttempts,

                    Time = (int?)i.Time,

                    NumOfTestRuns = (int)i.NumOfTestRuns,
                    NumOfSucceeded = (int)i.NumOfSucceeded,
                    NumOfTests = i.NumOfTests,
                }).ToArray();
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
    }
}
