using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class ExampleStatViewModel
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
        public string AvgTime { get; set; }
        public double StdDevTime { get; set; }
        public int NumOfAttempts { get; set; } = 1;

        public int NumOfTests { get; set; }
        public double AvgNumOfSucceeded { get; set; }

        public double AvgNumOfTestRuns { get; set; }

        public ExampleStatViewModel()
        {

        }
    }
}