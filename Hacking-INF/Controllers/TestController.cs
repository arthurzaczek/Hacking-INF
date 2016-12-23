using Hacking_INF.Models;
using Hacking_INF.Providers;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Hacking_INF.Controllers
{
    public class TestController : ApiController
    {
        private BL _bl;
        private readonly ILog _log = LogManager.GetLogger(typeof(TestController));
        private readonly log4net.ILog _logSubmission = log4net.LogManager.GetLogger("Submissions");
        private readonly SubmissionStoreProviderFactory _submissionStoreFactory;
        private readonly ITestResultSaveService _saveService;
        private static readonly Dictionary<Guid, TestOutput> _testOutput = new Dictionary<Guid, TestOutput>();
        private static readonly object _lock = new object();

        public TestController(BL bl, SubmissionStoreProviderFactory submissionStoreFactory, ITestResultSaveService saveService)
        {
            _bl = bl;
            _submissionStoreFactory = submissionStoreFactory;
            _saveService = saveService;
        }

        [HttpPost]
        public TestViewModel Test(TestViewModel vmdl)
        {
            var sessionGuid = Guid.Parse(vmdl.SessionID);

            // Fist, stop current execution
            lock (_lock)
            {
                TestOutput test;
                if (_testOutput.TryGetValue(sessionGuid, out test))
                {
                    _bl.KillProcessTree(test.Process);
                }
            }

            // Collect data
            var example = _bl.GetExamples(vmdl.Course).Single(i => i.Name == vmdl.Example);
            var course = _bl.GetCourses().Single(i => i.Name == vmdl.Course);
            var workingDir = _bl.GetWorkingDir(sessionGuid);
            var exampleDir = _bl.GetExampleDir(course.Name, example.Name);

            _log.InfoFormat("Testing {0}/{1}, session {2}", course.Name, example.Name, sessionGuid);

            // Cleanup
            _bl.CleanupWorkingDir(workingDir);

            // Save code
            var codeFileName = example.FileName ?? course.FileName;
            // Get optional main code
            var ourMainCode = "";
            var our_main = Directory.GetFiles(Path.Combine(exampleDir, "src"), "our_main.*").FirstOrDefault();
            if(our_main != null)
            {
                ourMainCode = Environment.NewLine + _bl.ReadTextFile(our_main);
            }

            _bl.WriteTextFile(Path.Combine(workingDir, codeFileName), vmdl.Code + ourMainCode);

            var user = _bl.GetCurrentUser();
            if (user != null)
            {
                // Optimization hint: use a lock object for each repository
                // this will prevent global locks.
                lock (_lock)
                {
                    var store = _submissionStoreFactory(course.Name, example.Name, user.UID);
                    store.Save(codeFileName, new System.IO.MemoryStream (Encoding.UTF8.GetBytes(vmdl.Code)));
                    store.Commit(string.Format("{0} submitted", codeFileName));
                    _log.InfoFormat("{0}.{1} commited by {2}", course.Name, example.Name, user.UID);
                    _logSubmission.InfoFormat("{0}.{1};{2};{3}", course.Name, example.Name, user.UID, _bl.GetClientIp(Request));
                }
            }

            // Compile
            var sb = new StringBuilder();
            var result = new TestViewModel();
            var failed = false;
            foreach (var compiler in example.Compiler ?? course.Compiler)
            {
                sb.AppendLine(compiler.Log);
                if (Exec(compiler.Cmd, compiler.Args, workingDir, sb) != 0)
                {
                    failed = true;
                }
            }
            result.CompileOutput = sb.ToString();
            result.CompileFailed = failed;

            // Test
            if (vmdl.CompileAndTest)
            {
                sb.Clear();
                File.Copy(Path.Combine(exampleDir, "properties.txt"), Path.Combine(workingDir, "properties.txt"));
                var args = string.Format("-Dexec=\"{0}\" -DtestFilesPath=\"{1}\" -DjunitOutFile=./results.xml -jar \"{2}\"",
                        example.Exe ?? course.Exe,
                        Path.Combine(exampleDir, "tests"),
                        Path.Combine(_bl.ToolsDir, "checkproject.jar"));

                Exec("java", args, workingDir, sessionGuid, vmdl.StartTime, user, course, example);
                result.TestFinished = false;
                result.TestOutput = "Starte Tests...";
            }
            else
            {
                // Just save the compile attempts
                _saveService.Save(user?.UID, sessionGuid, course, example, vmdl.StartTime);
            }

            return result;
        }

        public TestViewModel GetTestResult(string sessionID)
        {
            var sessionGuid = Guid.Parse(sessionID);
            lock (_lock)
            {
                // Remove zombie entires
                var dt = DateTime.Now.AddHours(-8);
                foreach (var kv in _testOutput.Where(i => i.Value.CreatedOn <= dt).ToList())
                {
                    _log.InfoFormat("Removing zombie session {0}", kv.Key);
                    _testOutput.Remove(kv.Key);
                }

                TestOutput test;
                if (_testOutput.TryGetValue(sessionGuid, out test))
                {
                    if (test.Process.HasExited)
                    {
                        _testOutput.Remove(sessionGuid);
                    }
                    return new TestViewModel() { SessionID = sessionID, TestFinished = false, TestOutput = test.Output.ToString() };
                }
                else
                {
                    return new TestViewModel() { SessionID = sessionID, TestFinished = true, TestOutput = "" };
                }
            }
        }

        private int Exec(string cmd, string args, string workingDir, StringBuilder sb)
        {
            var p = new Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.WorkingDirectory = workingDir;
            p.StartInfo.UseShellExecute = false;

            p.EnableRaisingEvents = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += (s, e) => sb.AppendLine(e.Data);
            p.ErrorDataReceived += (s, e) => sb.AppendLine(e.Data);

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            if (p.WaitForExit(10000))
            {
                _log.InfoFormat("Process {0} exited with error code {1}", cmd, p.ExitCode);
                return p.ExitCode;
            }
            else
            {
                _log.WarnFormat("Process {0} did not exited within 10 sec.");
                return 1; // generic fail.
            }
        }


        private void Exec(string cmd, string args, string workingDir, Guid sessionGuid, DateTime startTime, User user, Course course, Example example)
        {
            var p = new Process();
            var output = new TestOutput(p, user, sessionGuid, course, example, workingDir, startTime);
            lock (_lock)
            {
                _testOutput[sessionGuid] = output;
            }

            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.WorkingDirectory = workingDir;
            p.StartInfo.UseShellExecute = false;

            p.EnableRaisingEvents = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += (s, e) => { lock (_lock) output.Output.AppendLine(e.Data); };
            p.ErrorDataReceived += (s, e) => { lock (_lock) output.Output.AppendLine(e.Data); };
            p.Exited += (s, e) =>
            {
                _log.InfoFormat("Process {0} exited with error code {1}", cmd, p.ExitCode);
                _saveService.Save(output);
            };

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit(1);
        }
    }
}
