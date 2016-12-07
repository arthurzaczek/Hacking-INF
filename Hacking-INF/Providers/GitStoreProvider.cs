using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Configuration;

namespace Hacking_INF.Providers
{
    public abstract class GitStoreProvider : IDisposable
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger("Git");

        protected string course;
        protected string example;
        protected string assignmentName;
        protected string baseRelPath;
        protected string baseAbsPath;
        protected string repoPath;
        protected string user;
        protected Repository repo;

        public GitStoreProvider(string course, string example)
        {
            if (string.IsNullOrWhiteSpace(course)) throw new ArgumentNullException("course");
            if (string.IsNullOrWhiteSpace(example)) throw new ArgumentNullException("example");

            user = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            this.course = course;
            this.example = example;
        }

        protected abstract string GetRepoPath(string course, string example);

        protected void EnsureInitialization()
        {
            if (repo == null)
            {
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            assignmentName = example;
            repoPath = Path.GetFullPath(GetRepoPath(course, example));
            if (!Directory.Exists(repoPath))
            {
                Directory.CreateDirectory(repoPath);
                Repository.Init(repoPath);
            }
            repo = new Repository(repoPath);

            baseRelPath = GetLegalPathName(assignmentName) + Path.DirectorySeparatorChar;
            baseAbsPath = Path.Combine(repoPath, baseRelPath);
        }

        /// <summary>
        /// Replaces all illigal path characters with an '_' char.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetLegalPathName(string path)
        {
            Path.GetInvalidPathChars().ToList().ForEach(c => path = path.Replace(c, '_'));

            return path;
        }

        public virtual void Dispose()
        {
            repo.Dispose();
        }

        public void Save(string filename, System.IO.Stream content)
        {
            EnsureInitialization();

            var dest = Path.Combine(baseAbsPath, filename);
            var destPath = Path.GetDirectoryName(dest);
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            using (var fs = File.OpenWrite(dest))
            {
                fs.SetLength(0);
                content.CopyTo(fs);
                fs.Close();
            }
        }
        public virtual void Delete(string filename)
        {
            EnsureInitialization();

            if (string.IsNullOrWhiteSpace(filename))
            {
                foreach (var f in Directory.GetFiles(baseAbsPath))
                {
                    File.Delete(f);
                }
            }
            else
            {
                var filepath = Path.Combine(baseAbsPath, filename);
                File.Delete(filepath);
            }

            Signature author = new Signature(user, user + "@technikum-wien.at", DateTime.Now);
            var status = repo.RetrieveStatus();
            if (status.Missing.Count() > 0)
            {
                foreach (var toStage in status.Missing)
                {
                    repo.Stage(toStage.FilePath);
                }

                if (string.IsNullOrWhiteSpace(filename))
                {
                    repo.Commit(string.Format("{0}: Deleting all files", assignmentName), author, author);
                }
                else
                {
                    repo.Commit(string.Format("{0}: Deleting file '{1}'", assignmentName, filename), author, author);
                }
            }
        }

        public virtual void Commit(string message)
        {
            EnsureInitialization();

            Signature author = new Signature(user, user + "@technikum-wien.at", DateTime.Now);
            message = string.Format("{0}: {1}", assignmentName, message);
            var status = repo.RetrieveStatus();
            if (status.Count() > 0)
            {
                foreach (var toStage in status)
                {
                    repo.Stage(toStage.FilePath);
                }
                repo.Commit(message, author, author);
            }
            else
            {
                repo.Commit(message, author, author, new CommitOptions() { AllowEmptyCommit = true });
            }

        }

        public IEnumerable<SubmissionItem> GetItems(bool includeHiddenFiles = false)
        {
            EnsureInitialization();

            if (repo.Info.IsBare) yield break;

            foreach (var item in repo.Diff.Compare<TreeChanges>(new[] { baseRelPath }, includeUntracked: true, explicitPathsOptions: new ExplicitPathsOptions() { }, compareOptions: new CompareOptions() { IncludeUnmodified = true }))
            {
                var fileName = Path.GetFileName(item.Path);
                if (!includeHiddenFiles && StorageConstants.HiddenFiles.Contains(fileName.ToLower())) continue;
                yield return new GitSubmissionItem()
                {
                    Name = fileName,
                    FilePath = item.Path.Substring(baseRelPath.Length), // Cut off rel path prefix
                    FullFileName = Path.Combine(repoPath, item.Path),
                    Added = item.Status.HasFlag(ChangeKind.Added) || item.Status.HasFlag(ChangeKind.Untracked),
                    Deleting = item.Status.HasFlag(ChangeKind.Deleted),
                    Modified = item.Status.HasFlag(ChangeKind.Modified)
                };
            }
        }

        public IEnumerable<CommitItem> GetCommits()
        {
            EnsureInitialization();

            foreach (var item in repo.Commits.Take(20))
            {
                yield return new CommitItem()
                {
                    Message = item.MessageShort,
                    Author = item.Author.Name,
                    Email = item.Author.Email,
                    Timestamp = item.Author.When.LocalDateTime
                };
            }
        }
    }

    class GitSubmissionItem : SubmissionItem
    {
        internal string FullFileName { get; set; }

        public override Stream GetStream()
        {
            return new FileStream(FullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}