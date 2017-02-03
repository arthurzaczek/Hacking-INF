using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class ReportedCompilerMessageViewModel
    {
        public int ID { get; set; }

        public string Course { get; set; }
        public string CourseTitle { get; set; }
        public string Example { get; set; }
        public string ExampleTitle { get; set; }

        public string UID { get; set; }

        public DateTime Date { get; set; }
        public string Messages { get; set; }
        public string Code { get; set; }

        public ReportedCompilerMessageViewModel()
        {

        }

        public ReportedCompilerMessageViewModel(ReportedCompilerMessages obj)
        {
            Refresh(obj);
        }

        public void Refresh(ReportedCompilerMessages obj)
        {
            var target = this;
            var source = obj;

            target.ID = source.ID;
            target.Course = source.Course;
            target.Example = source.Example;

            target.Messages = source.Messages;
            target.Code = source.Code;
            target.Date = source.Date;

            target.UID = source.User?.UID;
        }
    }
}