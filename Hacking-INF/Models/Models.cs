using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public enum Types
    {
        NotDefined = 0,
        Open = 1,
        Closed = 2,
        Timed = 3,
    }

    public interface IStatus : ICloneable
    {
        Types Type { get; set; }
        DateTime? OpenFrom { get; set; }
        DateTime? OpenUntil { get; set; }
        DateTime? ClosedFrom { get; set; }
        DateTime? ClosedUntil { get; set; }
    }

    public class Course : IStatus
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        [YamlMember(Alias = "title")]
        public string Title { get; set; }
        [YamlMember(Alias = "headline")]
        public string HeadLine { get; set; }
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
        [YamlMember(Alias = "categories")]
        public List<Category> Categories { get; set; }
        [YamlMember(Alias = "type")]
        public Types Type { get; set; }
        [YamlMember(Alias = "openfrom")]
        public DateTime? OpenFrom { get; set; }
        [YamlMember(Alias = "openuntil")]
        public DateTime? OpenUntil { get; set; }
        [YamlMember(Alias = "closedfrom")]
        public DateTime? ClosedFrom { get; set; }
        [YamlMember(Alias = "closeduntil")]
        public DateTime? ClosedUntil { get; set; }

        [YamlMember(Alias = "exe")]
        public string Exe { get; set; }
        [YamlMember(Alias = "filename")]
        public string FileName { get; set; }
        [YamlMember(Alias = "compiler")]
        public List<Compiler> Compiler { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Compiler
    {
        [YamlMember(Alias = "cmd")]
        public string Cmd { get; set; }
        [YamlMember(Alias = "args")]
        public string Args { get; set; }
        [YamlMember(Alias = "log")]
        public string Log { get; set; }
    }

    public class Category
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        [YamlMember(Alias = "title")]
        public string Title { get; set; }
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
        [YamlMember(Alias = "examples")]
        public string[] Examples { get; set; }
    }

    public class Example : IStatus
    {
        [YamlMember(Alias = "problemshorttitle")]
        public string Name { get; set; }
        [YamlIgnore]
        public string Course { get; set; }
        [YamlMember(Alias = "problemtitle")]
        public string Title { get; set; }
        [YamlMember(Alias = "openfrom")]
        public DateTime? OpenFrom { get; set; }
        [YamlMember(Alias = "openuntil")]
        public DateTime? OpenUntil { get; set; }
        [YamlMember(Alias = "closedfrom")]
        public DateTime? ClosedFrom { get; set; }
        [YamlMember(Alias = "closeduntil")]
        public DateTime? ClosedUntil { get; set; }

        [YamlMember(Alias = "description")]
        public string Description { get; set; }
        [YamlMember(Alias = "type")]
        public Types Type { get; set; }
        [YamlMember(Alias = "category")]
        public string Category { get; set; }
        [YamlMember(Alias = "difficulty")]
        public string Difficulty { get; set; }
        [YamlMember(Alias = "requires")]
        public List<string> Requires { get; set; }

        // Overrides
        [YamlMember(Alias = "exe")]
        public string Exe { get; set; }
        [YamlMember(Alias = "filename")]
        public string FileName { get; set; }
        [YamlMember(Alias = "compiler")]
        public List<Compiler> Compiler { get; set; }
        public int Order { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void InheritProperties(Course courseObj)
        {
            // Inheritance
            if (this.Type == Types.NotDefined)
                this.Type = courseObj.Type;
            if (this.OpenFrom == null)
                this.OpenFrom = courseObj.OpenFrom;
            if (this.OpenUntil == null)
                this.OpenUntil = courseObj.OpenUntil;
            if (this.ClosedFrom == null)
                this.ClosedFrom = courseObj.ClosedFrom;
            if (this.ClosedUntil == null)
                this.ClosedUntil = courseObj.ClosedUntil;

            if (string.IsNullOrWhiteSpace(this.FileName))
                this.FileName = courseObj.FileName;

            if (string.IsNullOrWhiteSpace(this.Exe))
                this.Exe = courseObj.Exe;
        }
    }

    public class TestOutput : IDisposable
    {
        string _workingDir;
        public TestOutput(Process p, string userUID, Guid sessionID, Course course, Example example, string workingDir, DateTime startTime)
        {
            this._workingDir = workingDir;
            this.Process = p;
            this.UID = userUID;
            this.SessionID = sessionID;
            this.Course = course.Name;
            this.Example = example.Name;
            this.StartTime = startTime;
            this.XUnitFile = System.IO.Path.Combine(workingDir, "results.xml");
        }
        public StringBuilder Output { get; } = new StringBuilder();
        public Process Process { get; private set; }
        public DateTime CreatedOn { get; } = DateTime.Now;
        public string XUnitFile { get; private set; }
        public string Course { get; private set; }
        public string Example { get; private set; }
        public string UID { get; private set; }
        public Guid SessionID { get; private set; }
        public DateTime StartTime { get; private set; }
        public bool IsFinished { get; private set; }

        public int NumOfTests { get; private set; }
        public int NumOfErrors { get; private set; }
        public int NumOfFailed { get; private set; }
        public int NumOfSkipped { get; private set; }
        public int NumOfSucceeded { get; private set; }

        public List<MemoryErrors> MemoryErrors { get; private set; } = new List<MemoryErrors>();

        public void Dispose()
        {
            Process?.Dispose();
        }
        public void Finish()
        {
            IsFinished = true;
            if (System.IO.File.Exists(XUnitFile))
            {
                var xml = new XmlDocument();
                xml.Load(XUnitFile);
                var node = xml.SelectSingleNode("//testsuite | //test-results");
                if (node != null)
                {
                    NumOfTests = int.Parse(node.Attributes["total"]?.Value ?? "0") + int.Parse(node.Attributes["tests"]?.Value ?? "0");
                    NumOfErrors = int.Parse(node.Attributes["errors"]?.Value ?? "0");
                    NumOfFailed = int.Parse(node.Attributes["failures"]?.Value ?? "0");
                    NumOfSkipped = int.Parse(node.Attributes["skipped"]?.Value ?? "0");
                    NumOfSucceeded = NumOfTests - NumOfErrors - NumOfFailed;
                }
            }

            foreach (var mem in System.IO.Directory.GetFiles(_workingDir, "memoryreport-*.log"))
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(mem).Substring(13); // - memoryreport-
                name = name.Substring(0, name.Length - 3); // - .in
                using (var sr = new System.IO.StreamReader(mem))
                {
                    MemoryErrors.Add(new MemoryErrors()
                    {
                        TestCase = name,
                        Report = sr.ReadToEnd(),
                    });
                }

            }
        }
    }

    public class MemoryErrors
    {
        public string TestCase { get; set; }
        public string Report { get; set; }
    }

    public class CompilerMessage
    {
        [YamlMember(Alias = "message")]
        public string Message { get; set; }
        [YamlMember(Alias = "hint")]
        public string Hint { get; set; }
    }

    [Table("Users")]
    public class User
    {
        public User()
        {
            ExampleResults = new List<ExampleResult>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string UID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ExampleResult> ExampleResults { get; set; }
    }

    [Table("ExampleResults")]
    public class ExampleResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public virtual User User { get; set; }
        public Guid? SessionID { get; set; }

        [Required]
        public string Course { get; set; }
        [Required]
        public string Example { get; set; }

        [Required]
        public DateTime FirstAttempt { get; set; }
        [Required]
        public DateTime LastAttempt { get; set; }
        /// <summary>
        /// Time needed in seconds
        /// </summary>
        public int? Time { get; set; }

        public int NumOfTests { get; set; }
        public int NumOfSucceeded { get; set; }
        public int NumOfFailed { get; set; }
        public int NumOfErrors { get; set; }
        public int NumOfSkipped { get; set; }

        public int NumOfCompilations { get; set; }
        public int NumOfTestRuns { get; set; }

        public int? LinesOfCode { get; set; }
        public int? CyclomaticComplexity { get; set; }
        public int? MemErrors { get; set; }

    }

    [Table("ReportedCompilerMessages")]
    public class ReportedCompilerMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string Course { get; set; }
        [Required]
        public string Example { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Messages { get; set; }
        public string Code { get; set; }
    }

    public class ExamplesRepo
    {
        [YamlMember(Alias = "url")]
        public string Url { get; set; }
        [YamlMember(Alias = "user")]
        public string User { get; set; }
        [YamlMember(Alias = "pwd")]
        public string Pwd { get; set; }
    }
}