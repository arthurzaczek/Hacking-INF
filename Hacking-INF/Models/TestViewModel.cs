using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class TestViewModel
    {
        public string Course { get; set; }
        public string Example { get; set; }
        public string SessionID { get; set; }
        public DateTime StartTime { get; set; }
        public string Code { get; set; }
        public bool CompileAndTest { get; set; }

        public string CompileOutput { get; set; }
        public bool CompileFailed { get; set; }
        public string TestOutput { get; set; }
        public MemoryErrorsViewModel[] MemoryErrors { get; set; } = new MemoryErrorsViewModel[] { };
        public bool TestFinished { get; set; }

        public int NumOfTests { get; set; }
        public int NumOfSucceeded { get; set; }

        public bool Succeeded
        {
            get
            {
                return NumOfTests > 0 && NumOfTests == NumOfSucceeded;
            }
        }

        public TestViewModel()
        {

        }
    }
}