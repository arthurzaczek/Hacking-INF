using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Hacking_INF.Providers
{
    public delegate ISubmissionStoreProvider SubmissionStoreProviderFactory(string course, string exampleRegex, string uid);
    public delegate IEnumerable<ISubmissionStoreProvider> SubmissionCollectorFactory(string course, string exampleRegex);

    public sealed class StorageConstants
    {
        public static readonly string[] HiddenFiles = new[] { "jenkins.zip", "solution.zip" };
    }

    public abstract class SubmissionItem
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool Added { get; set; }
        public bool Deleting { get; set; }
        public bool Modified { get; set; }

        public abstract Stream GetStream();
    }

    public class CommitItem
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public string Author { get; set; }
        public string Email { get; set; }
    }

    public interface IStoreProvider
    {
        void Save(string filename, Stream content);
        void Delete(string filename);
        void Commit(string message);
        IEnumerable<SubmissionItem> GetItems(bool includeHidenFiles = false);
        IEnumerable<CommitItem> GetCommits();
    }

    public interface ISubmissionStoreProvider : IStoreProvider
    {
        string UID { get; }
    }
}