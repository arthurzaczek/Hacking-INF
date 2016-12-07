using Autofac;
using Hacking_INF.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hacking_INF
{
    public class HackingModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<BL>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<GitSubmissionStoreProvider>()
                .As<ISubmissionStoreProvider>()
                .InstancePerDependency();

            builder.RegisterType<TestResultSaveService>()
                .As<ITestResultSaveService>()
                .SingleInstance();

            builder.Register<IEnumerable<ISubmissionStoreProvider>>((c, p) => GitSubmissionStoreProvider.GetSubmissions(p.Named<string>("course"), p.Named<string>("example")))
                .As<IEnumerable<ISubmissionStoreProvider>>()
                .InstancePerDependency();
        }
    }
}