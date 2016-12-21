using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class ExampleResultViewModel
    {
        public int ID { get; set; }

        public string Course { get; set; }
        public string CourseTitle { get; set; }
        public string Example { get; set; }
        public string ExampleTitle { get; set; }

        public DateTime FirstAttempt { get; set; }
        public DateTime LastAttempt { get; set; }
        /// <summary>
        /// Time needed in seconds
        /// </summary>
        public int? Time { get; set; }

        public int NumOfTests { get; set; }
        public int NumOfSucceeded { get; set; }
        public int NumOfFailed { get; set; }
        public int NumOfErrors { get; set; }
        public int NumOfSkipped { get; set; }

        public int NumOfCompilations { get; set; }
        public int NumOfTestRuns { get; set; }

        public int? LinesOfCode { get; set; }
        public int? CyclomaticComplexity { get; set; }
        public int? MemErrors { get; set; }

        public ExampleResultViewModel()
        {

        }

        public ExampleResultViewModel(ExampleResult obj)
        {
            Refresh(obj);
        }

        public void Refresh(ExampleResult obj)
        {
            var target = this;
            var source = obj;

            target.Course = source.Course;
            target.Example = source.Example;

            target.FirstAttempt = source.FirstAttempt;
            target.LastAttempt = source.LastAttempt;
            target.Time = source.Time;

            target.NumOfCompilations = source.NumOfCompilations;
            target.NumOfErrors = source.NumOfErrors;
            target.NumOfFailed = source.NumOfFailed;
            target.NumOfSkipped = source.NumOfSkipped;
            target.NumOfSucceeded = source.NumOfSucceeded;
            target.NumOfTestRuns = source.NumOfTestRuns;
            target.NumOfTests = source.NumOfTests;

            target.MemErrors = source.MemErrors;
            target.CyclomaticComplexity = source.CyclomaticComplexity;
            target.LinesOfCode = source.LinesOfCode;
        }
    }
}