using Hacking_INF.Models;
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
        private BL _bl = new BL();
        [HttpPost]
        public TestViewModel Test(TestViewModel vmdl)
        {
            var sessionGuid = Guid.Parse(vmdl.SessionID);
            var example = _bl.GetExamples(vmdl.Course).Single(i => i.Name == vmdl.Example);
            var course = _bl.GetCourses().Single(i => i.Name == vmdl.Course);
            var workingDir = _bl.GetWorkingDir(sessionGuid);
            var exampleDir = _bl.GetExampleDir(course.Name, example.Name);

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(workingDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo subdir in di.GetDirectories())
                {
                    subdir.Delete(true);
                }
            }

            _bl.WriteTextFile(Path.Combine(workingDir, course.FileName), vmdl.Code);
            var sb = new StringBuilder();

            var result = new TestViewModel();
            var failed = false;
            foreach (var compiler in course.Compiler)
            {
                sb.AppendLine(compiler.Log);
                if(Exec(compiler.Cmd, compiler.Args, workingDir, sb) != 0)
                {
                    failed = true;
                }
            }
            result.CompileOutput = sb.ToString();
            result.CompileFailed = failed;

            if (vmdl.CompileAndTest)
            {
                // TODO: Use Async call...
                sb.Clear();
                File.Copy(Path.Combine(exampleDir, "properties.txt"), Path.Combine(workingDir, "properties.txt"));
                var args = string.Format("-Dexec={0} -DtestFilesPath=\"{1}\" -jar \"{2}\"",
                        course.Exe,
                        Path.Combine(exampleDir, "tests"),
                        Path.Combine(_bl.ToolsDir, "checkproject.jar"));

                result.TestFailed = Exec("java", args, workingDir, sb) != 0;
                result.TestOutput = sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// Use this only for sync process calls, TODO: Implement async call
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <param name="workingDir"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
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
    }
}
