using Autofac;
using Hacking_INF.Controllers;
using Hacking_INF.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Hacking_INF
{
    public interface ILoadTest
    {
        void Run(int concurrent, int numberOfTests);
    }

    public class LoadTest : ILoadTest
    {
        private int _concurrent;
        private int _numberOfTests;
        private TestController _testController;
        private readonly ILog _log = LogManager.GetLogger(typeof(LoadTest));
        private int progress;

        public LoadTest()
        {
        }

        public void Run(int concurrent, int numberOfTests)
        {
            _log.Info($"Starting load tests with {concurrent} users and {numberOfTests} # of tests");
            _concurrent = concurrent;
            _numberOfTests = numberOfTests;
            progress = _numberOfTests * _concurrent;

            new Thread(_ =>
            {
                var lst = new List<Thread>();

                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                using (var scope = Autofac.Integration.Mvc.AutofacDependencyResolver.Current.ApplicationContainer.BeginLifetimeScope())
                {
                    _testController = scope.Resolve<TestController>();

                    for (int i = 0; i < concurrent; i++)
                    {
                        var t = new Thread(__ => DoRequests(i + 1));
                        t.Start();
                        lst.Add(t);
                    }

                    lst.ForEach(t => t.Join());

                    watch.Stop();
                    _log.InfoFormat($"Load Test with {concurrent} users and {numberOfTests} # of tests ended after {(watch.ElapsedMilliseconds / 1000.0m).ToString("n3")} seconds.");
                }
            }).Start();
        }

        void DoRequests(int num)
        {
            for (int i = 0; i < _numberOfTests; i++)
            {
                try
                {
                    var watch = new System.Diagnostics.Stopwatch();
                    watch.Start();
                    _log.Info($"Working on load test thread {num}/{i + 1}: ");

                    var vmdl = new TestViewModel();
                    vmdl.Course = "_C";
                    vmdl.Example = "HelloWorld";
                    vmdl.Code = @"#include <stdio.h>
#include <stdlib.h>

int main()
{
	printf(""Hello World"");
    return 0;
}";
                    vmdl.SessionID = Guid.NewGuid().ToString();
                    vmdl.CompileAndTest = true;

                    _testController.Test(vmdl);

                    var counter = 0;
                    while (counter++ < 120 * 10)
                    {
                        var result = _testController.GetTestResult(vmdl.SessionID);
                        if (result.TestFinished) break;
                        Thread.Sleep(100);
                    }

                    watch.Stop();
                    _log.Info($"    {num}/{i + 1}: took {(watch.ElapsedMilliseconds / 1000.0m).ToString("n3")} seconds.");
                }
                catch (Exception ex)
                {
                    _log.Error($"Error on load test thread {num}/{i+1}", ex);
                }

                var tmp = Interlocked.Decrement(ref progress);
                _log.Info($"Progress: {tmp} until finished.");
            }
        }
    }
}
