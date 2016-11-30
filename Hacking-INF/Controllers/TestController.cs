﻿using Hacking_INF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Hacking_INF.Controllers
{
    public class TestController : ApiController
    {
        private BL _bl = new BL();
        private static readonly Dictionary<Guid, TestOutput> _testOutput = new Dictionary<Guid, TestOutput>();
        private static readonly object _lock = new object();

        public class TestOutput
        {
            public TestOutput(Process p)
            {
                this.Process = p;
            }
            public StringBuilder Output { get; } = new StringBuilder();
            public Process Process { get; private set; }
            public DateTime CreatedOn { get; } = DateTime.Now;
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

            // Cleanup
            _bl.CleanupWorkingDir(workingDir);

            // Save code
            _bl.WriteTextFile(Path.Combine(workingDir, course.FileName), vmdl.Code);

            // Compile
            var sb = new StringBuilder();
            var result = new TestViewModel();
            var failed = false;
            foreach (var compiler in course.Compiler)
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
                var args = string.Format("-Dexec={0} -DtestFilesPath=\"{1}\" -jar \"{2}\"",
                        course.Exe,
                        Path.Combine(exampleDir, "tests"),
                        Path.Combine(_bl.ToolsDir, "checkproject.jar"));

                Exec("java", args, workingDir, sessionGuid);
                result.TestFinished = false;
                result.TestOutput = "Starte Tests...";
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

        private static int Exec(string cmd, string args, string workingDir, StringBuilder sb)
        {
            var p = new Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.WorkingDirectory = workingDir;
            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += (s, e) => sb.AppendLine(e.Data);
            p.ErrorDataReceived += (s, e) => sb.AppendLine(e.Data);

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            if (p.WaitForExit(10000))
            {
                return p.ExitCode;
            }
            else
            {
                return 1; // generic fail.
            }
        }


        private static void Exec(string cmd, string args, string workingDir, Guid sessionGuid)
        {

            var p = new Process();
            var output = new TestOutput(p);
            lock (_lock)
            {
                _testOutput[sessionGuid] = output;
            }

            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.WorkingDirectory = workingDir;
            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += (s, e) => { lock (_lock) output.Output.AppendLine(e.Data); };
            p.ErrorDataReceived += (s, e) => { lock (_lock) output.Output.AppendLine(e.Data); };

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit(1);
        }
    }
}
