using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class StudentStatViewModel
    {
        public string User { get; set; }
        public string UID { get; set; }

        public DateTime FirstAttempt { get; set; }
        public DateTime LastAttempt { get; set; }
        /// <summary>
        /// Time needed as string
        /// </summary>
        public string Time { get; set; }

        public int NumOfTests { get; set; }
        public double NumOfSucceeded { get; set; }

        public double NumOfTestRuns { get; set; }

        public StudentStatDetailViewModel[] Details { get; set; }
    }
    public class StudentStatDetailViewModel
    {
        public string Course { get; set; }
        public string CourseTitle { get; set; }
        public string Example { get; set; }
        public string ExampleTitle { get; set; }

        public DateTime FirstAttempt { get; set; }
        public DateTime LastAttempt { get; set; }
        /// <summary>
        /// Time needed as string
        /// </summary>
        public string Time { get; set; }

        public int NumOfTests { get; set; }
        public double NumOfSucceeded { get; set; }

        public double NumOfTestRuns { get; set; }

        public StudentStatDetailViewModel()
        {

        }
    }
}