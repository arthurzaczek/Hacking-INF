using Autofac;
using Hacking_INF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Hacking_INF.Providers
{
    public interface ITestResultSaveService
    {
        void Save(TestOutput output);
        void Save(User user, Course course, Example example, DateTime startTime);
    }

    public class TestResultSaveService : ITestResultSaveService
    {
        private readonly ILifetimeScope _rootScope;

        public TestResultSaveService(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }


        public void Save(TestOutput output)
        {
            if (string.IsNullOrWhiteSpace(output.UID)) return; // nothing to save

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var bl = scope.Resolve<BL>();
                var user = bl.GetUser(output.UID);
                var result = bl.GetExampleResult(user, output.Course, output.Example);
                if(result == null)
                {
                    result = bl.CreateExampleResult();
                    result.User = user;
                    result.Course = output.Course;
                    result.Example = output.Example;
                    result.FirstAttempt = output.CreatedOn;
                    result.NumOfCompilations = 0;
                    result.NumOfTestRuns = 0;
                }

                result.NumOfCompilations++;
                result.NumOfTestRuns++;

                result.Time = (int)(DateTime.Now - output.StartTime).TotalSeconds;
                result.LastAttempt = output.CreatedOn;

                if(System.IO.File.Exists(output.XUnitFile))
                {
                    var xml = new XmlDocument();
                    xml.Load(output.XUnitFile);
                    var node = xml.SelectSingleNode("//testsuite | //test-results");
                    if(node != null)
                    {
                        result.NumOfTests = int.Parse(node.Attributes["total"]?.Value ?? "0") + int.Parse(node.Attributes["tests"]?.Value ?? "0");
                        result.NumOfErrors = int.Parse(node.Attributes["errors"]?.Value ?? "0");
                        result.NumOfFailed = int.Parse(node.Attributes["failures"]?.Value ?? "0");
                        result.NumOfSkipped = int.Parse(node.Attributes["skipped"]?.Value ?? "0");
                        result.NumOfSucceeded = result.NumOfTests - result.NumOfErrors - result.NumOfFailed;
                    }
                }

                bl.SaveChanges();
            }
        }

        public void Save(User user, Course course, Example example, DateTime startTime)
        {
            if (user == null) return; // nothing to save

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var bl = scope.Resolve<BL>();
                var result = bl.GetExampleResult(user, course.Name, example.Name);
                if (result == null)
                {
                    result = bl.CreateExampleResult();
                    result.User = user;
                    result.Course = course.Name;
                    result.Example = example.Name;
                    result.FirstAttempt = DateTime.Now;
                    result.NumOfCompilations = 0;
                    result.NumOfTestRuns = 0;
                }

                result.LastAttempt = DateTime.Now;

                result.Time = (int)(DateTime.Now - startTime).TotalSeconds;
                result.NumOfCompilations++;

                // no unit test results

                bl.SaveChanges();
            }
        }
    }
}