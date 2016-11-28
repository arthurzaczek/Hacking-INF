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
            var mdl = _bl.GetExamples(vmdl.Course).Single(i => i.Name == vmdl.Example);
            var dir = _bl.GetWorkingDir(sessionGuid);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            _bl.WriteTextFile(Path.Combine(dir, "main.c"), vmdl.Code);
            var sb = new StringBuilder();

            sb.AppendLine("Compiling");
            sb.AppendLine("----------------------");
            Exec("gcc", "-Wall -g -pedantic -Wextra -std=c99 -ggdb -static-libgcc -c main.c", dir, sb);
            sb.AppendLine("Linking");
            sb.AppendLine("----------------------");
            Exec("gcc", "-o main.exe main.o", dir, sb);

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
