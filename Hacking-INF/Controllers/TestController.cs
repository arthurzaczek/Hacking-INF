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
            var dir = _bl.GetWorkingDir(sessionGuid);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            _bl.WriteTextFile(Path.Combine(dir, course.FileName), vmdl.Code);
            var sb = new StringBuilder();

            foreach (var compiler in course.Compiler)
            {
                sb.AppendLine(compiler.Log);
                Exec(compiler.Cmd, compiler.Args, dir, sb);
            }

            return new TestViewModel() { CompileOutput = sb.ToString() };
        }

        private static void Exec(string cmd, string args, string workingDir, StringBuilder sb)
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
            p.WaitForExit(10000);
        }
    }
}
