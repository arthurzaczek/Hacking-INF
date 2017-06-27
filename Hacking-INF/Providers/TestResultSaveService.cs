using Autofac;
using Hacking_INF.Models;
using log4net;
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
        void Save(string uid, Guid? sessionID, Course course, Example example, DateTime startTime);
    }

    public class TestResultSaveService : ITestResultSaveService
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(TestResultSaveService));
        private readonly ILifetimeScope _rootScope;

        public TestResultSaveService(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }


        public void Save(TestOutput output)
        {
            try
            {
                _log.InfoFormat("Saving {0}/{1} for {2}", output.Course, output.Example, !string.IsNullOrWhiteSpace(output.UID) ? (object)output.UID : (object)output.SessionID);
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var bl = scope.Resolve<BL>();
                    var user = !string.IsNullOrWhiteSpace(output.UID) ? bl.GetUser(output.UID, checkAccess: false) : null;
                    var sessionID = output.SessionID;

                    var result = bl.GetExampleResult(user?.UID, sessionID, output.Course, output.Example);
                    if (result == null)
                    {
                        result = bl.CreateExampleResult();
                        result.User = user;
                        result.SessionID = user == null ? sessionID : (Guid?)null;
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

                    result.NumOfTests = output.NumOfTests;
                    result.NumOfErrors = output.NumOfErrors;
                    result.NumOfFailed = output.NumOfFailed;
                    result.NumOfSkipped = output.NumOfSkipped;
                    result.NumOfSucceeded = output.NumOfSucceeded;

                    bl.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Error saving {0}/{1} for {2}", output.Course, output.Example, !string.IsNullOrWhiteSpace(output.UID) ? (object)output.UID : (object)output.SessionID), ex);
                throw;
            }
        }

        public void Save(string uid, Guid? sessionID, Course course, Example example, DateTime startTime)
        {
            try
            {
                _log.InfoFormat("Saving {0}/{1} for {2}", course, example, !string.IsNullOrWhiteSpace(uid) ? (object)uid : sessionID);
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var bl = scope.Resolve<BL>();
                    var user = !string.IsNullOrWhiteSpace(uid) ? bl.GetUser(uid, checkAccess: false) : null;
                    var result = bl.GetExampleResult(user?.UID, sessionID, course.Name, example.Name);
                    if (result == null)
                    {
                        result = bl.CreateExampleResult();
                        result.User = user;
                        result.SessionID = user == null ? sessionID : null;
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
            catch (Exception ex)
            {
                _log.Error(string.Format("Error saving {0}/{1} for {2}", course, example, !string.IsNullOrWhiteSpace(uid) ? (object)uid : sessionID), ex);
                throw;
            }
        }
    }
}