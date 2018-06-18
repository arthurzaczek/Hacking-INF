using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Hacking_INF.Providers
{
    public class GitSubmissionStoreProvider : GitStoreProvider, ISubmissionStoreProvider
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger("Git");

        private readonly string uid;

        public GitSubmissionStoreProvider(string course, string exampleRegex, string uid)
            : base(course, exampleRegex)
        {
            if (string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException("uid");
            this.uid = uid;
        }

        private static string _SubmissionsDir;
        public static string SubmissionsDir
        {
            get
            {
                return _SubmissionsDir ?? (_SubmissionsDir = HostingEnvironment.MapPath("~/App_Data/Submissions"));
            }
        }

        protected override string GetRepoPath(string course)
        {
            return Path.Combine(
                SubmissionsDir,
                "~" + uid,
                GetLegalPathName(course) + ".git"
            );
        }


        public static IEnumerable<ISubmissionStoreProvider> GetSubmissions(string course, string example)
        {
            var couseName = GetLegalPathName(course) + ".git";
            foreach (var path in Directory.GetDirectories(SubmissionsDir))
            {
                var uid = Path.GetFileName(path); // Not GetDirectoryName!
                if (!uid.StartsWith("~")) continue;
                uid = uid.Substring(1);
                var probePath = Path.Combine(path, couseName);
                if (Directory.Exists(probePath))
                {
                    yield return new GitSubmissionStoreProvider(course, example, uid);
                }
            }
        }


        public string UID
        {
            get
            {
                return uid;
            }
        }

        public override void Commit(string message)
        {
            base.Commit(message);
        }

        public override void Delete(string filename)
        {
            base.Delete(filename);
        }
    }
}
