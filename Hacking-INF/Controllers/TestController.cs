using Hacking_INF.Models;
using Hacking_INF.Providers;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var example = _bl.GetExamples(vmdl.Course).Single(i => Path.GetFileName(i.Name) == vmdl.Example);
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
            if (our_main != null)
            {
                ourMainCode = Environment.NewLine + _bl.ReadTextFile(our_main);
            }

            _bl.WriteTextFile(Path.Combine(workingDir, codeFileName), vmdl.Code + ourMainCode);

            var userUID = _bl.GetCurrentUserUID();
            if (!string.IsNullOrWhiteSpace(userUID))
            {
                // Optimization hint: use a lock object for each repository
                // this will prevent global locks.
                lock (_lock)
                {
                    var store = _submissionStoreFactory(course.Name, Path.GetFileName(example.Name), userUID);
                    store.Save(codeFileName, new System.IO.MemoryStream(Encoding.UTF8.GetBytes(vmdl.Code)));
                    store.Commit(string.Format("{0} submitted", codeFileName));
                    _log.InfoFormat("{0}.{1} commited by {2}", course.Name, example.Name, userUID);
                    _logSubmission.InfoFormat("{0}.{1};{2};{3}", course.Name, example.Name, userUID, _bl.GetClientIp(Request));
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

            if (failed)
            {
                var rcm = _bl.CreateReportedCompilerMessages();
                rcm.Code = vmdl.Code;
                rcm.Course = course.Name;
                rcm.Example = example.Name;
                rcm.Messages = result.CompileOutput;

                _bl.CleanupOldReportedCompilerMessages();

                _bl.SaveChanges();
            }

            // Test
            if (vmdl.CompileAndTest && !failed)
            {
                sb.Clear();
                File.Copy(Path.Combine(exampleDir, "properties.txt"), Path.Combine(workingDir, "properties.txt"));
                // Copy everything except testfiles from the tests folder. 
                // There may be test files for file I/O
                foreach(var f in Directory.GetFiles(Path.Combine(exampleDir, "tests"), "*.*").Where(f => !f.EndsWith(".in") && !f.EndsWith(".sexp") && !f.EndsWith(".fexp")))
                {
                    File.Copy(f, Path.Combine(workingDir, Path.GetFileName(f)));
                }
                    
                var args = string.Format("-Dexec=\"{0}\" -DtestFilesPath=\"{1}\" -DjunitOutFile=./results.xml -DdrMemoryPath=\"{2}\" -jar \"{3}\"",
                        example.Exe ?? course.Exe,
                        Path.Combine(exampleDir, "tests"),
                        Properties.Settings.Default.DrMemoryPath,
                        Path.Combine(HackingEnvironment.Current.ToolsDir, "checkproject.jar"));

                Exec("java", args, workingDir, sessionGuid, vmdl.StartTime, userUID, course, example);
                result.TestFinished = false;
                result.TestOutput = "Starte Tests...";
            }
            else
            {
                // Just save the compile attempts
                _saveService.Save(userUID, sessionGuid, course, example, vmdl.StartTime);
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
                    kv.Value.Dispose();
                    _testOutput.Remove(kv.Key);
                }

                TestOutput test;
                if (_testOutput.TryGetValue(sessionGuid, out test))
                {
                    if (test.IsFinished)
                    {
                        test.Dispose();
                        _testOutput.Remove(sessionGuid);
                    }
                    return new TestViewModel()
                    {
                        SessionID = sessionID,
                        TestFinished = test.IsFinished,
                        TestOutput = test.Output.ToString(),
                        NumOfTests = test.NumOfTests,
                        NumOfSucceeded = test.NumOfSucceeded,
                        MemoryErrors = test.MemoryErrors.Select(i => new MemoryErrorsViewModel(i)).ToArray(),
                    };
                }
                else
                {
                    return new TestViewModel() { SessionID = sessionID, TestFinished = true, TestOutput = "" };
                }
            }
        }

        private int Exec(string cmd, string args, string workingDir, StringBuilder sb)
        {
            using (var p = new Process())
            {
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
                    _log.InfoFormat("Process \"{0} {1}\" exited with error code {2}", cmd, args, p.ExitCode);
                    return p.ExitCode;
                }
                else
                {
                    _log.WarnFormat("Process \"{0} {1}\" did not exited within 10 sec.", cmd, args);
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception ex)
                    {
                        _log.Warn("  unable to kill process", ex);
                    }

                    return 1; // generic fail.
                }
            }
        }


        private void Exec(string cmd, string args, string workingDir, Guid sessionGuid, DateTime startTime, string userUID, Course course, Example example)
        {
            var p = new Process();
            var output = new TestOutput(p, userUID, sessionGuid, course, example, workingDir, startTime);
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
                _log.InfoFormat("Process \"{0} {1}\" exited with error code {2}", cmd, args, p.ExitCode);
                output.Finish();
                _saveService.Save(output);
                p.Dispose();
            };

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit(1);
        }
    }
}
