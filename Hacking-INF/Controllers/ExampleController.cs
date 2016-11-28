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
    public class ExampleController : ApiController
    {
        private BL _bl = new BL();
        public string Compile(string course, string example, string sessionID, string code)
        {
            var sessionGuid = Guid.Parse(sessionID);
            var mdl = _bl.GetExamples(course).Single(i => i.Name == example);
            var dir = _bl.GetWorkingDir(sessionGuid);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            _bl.WriteTextFile(Path.Combine(dir, "main.c"), code);

            var si = new ProcessStartInfo("gcc", "-Wall -g -pedantic -Wextra -std=c99 -ggdb -static-libgcc -c main.c");
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            var p = Process.Start(si);
            p.WaitForExit(10000);

            var sb = new StringBuilder();
            sb.AppendLine(p.StandardOutput.ReadToEnd());

            si = new ProcessStartInfo("gcc", "-o main.exe main.o");
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            p = Process.Start(si);
            p.WaitForExit(10000);

            sb.AppendLine(p.StandardOutput.ReadToEnd());

            return sb.ToString(); 
        }
    }
}
