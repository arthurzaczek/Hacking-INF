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
        public HttpResponseMessage Download(string course, string example)
        {
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
    }
}
