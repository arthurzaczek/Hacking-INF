using Hacking_INF.Controllers;
using Hacking_INF.Models;
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
        int _concurrent;
        int _numberOfTests;
        private TestController _testController;

        public LoadTest(TestController testController)
        {
            _testController = testController;
        }

        public void Run(int concurrent, int numberOfTests)
        {
            _concurrent = concurrent;
            _numberOfTests = numberOfTests;

            for(int i=0;i<concurrent;i++)
            {
                new Thread(_ => DoRequests()).Start();
            }
        }

        void DoRequests()
        {
            for(int i=0;i<_numberOfTests;i++)
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
    }
}
