using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private string _compileOutputFormatted = null;
        public string CompileOutputFormatted
        {
            get
            {
                if(_compileOutputFormatted == null && !string.IsNullOrWhiteSpace(this.CompileOutput))
                {
                    var sbResult = new StringBuilder();
                    using (var sr = new StringReader(this.CompileOutput))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var test = line.ToLower();
                            if (test.Contains("error"))
                            {
                                sbResult.AppendLine("<span class=\"compiler error\">" + line + "</span>");
                            }
                            else if (test.Contains("warning") || test.Contains("warn"))
                            {
                                sbResult.AppendLine("<span class=\"compiler warning\">" + line + "</span>");
                            }
                            else
                            {
                                sbResult.AppendLine(line);
                            }
                        }
                    }
                    _compileOutputFormatted = sbResult.ToString();
                }
                return _compileOutputFormatted ?? string.Empty;
            }
            set
            {
                if (_compileOutputFormatted != value)
                {
                    _compileOutputFormatted = value;
                }
            }
        }

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