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

        public LoadTest()
        {
        }

        public void Run(int concurrent, int numberOfTests)
        {
            _concurrent = concurrent;
            _numberOfTests = numberOfTests;

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
                        var t = new Thread(__ => DoRequests());
                        t.Start();
                        lst.Add(t);
                    }

                    lst.ForEach(t => t.Join());

                    watch.Stop();
                    _log.InfoFormat($"Load Test with {concurrent} users and {numberOfTests} # of tests ended after {(watch.ElapsedMilliseconds / 1000.0m).ToString("n3")} seconds.");
                }
            }).Start();
        }

        void DoRequests()
        {
            try
            {
                for (int i = 0; i < _numberOfTests; i++)
                {
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
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error on one load test thread", ex);
            }
        }
    }
}
