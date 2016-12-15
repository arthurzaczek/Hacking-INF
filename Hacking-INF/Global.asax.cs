using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Hacking_INF
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(WebApiApplication));
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            
            _log.Info("** Starting Application **");

            BuildMasterContainer();
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void BuildMasterContainer()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(WebApiApplication).Assembly);
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly); 

            builder.RegisterModule<EntityFrameworkModule>();
            builder.RegisterModule<HackingModule>();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        public override void Init()
        {
            base.Init();
            this.Error += new EventHandler(WebApiApplication_Error);
        }

        void WebApiApplication_Error(object sender, EventArgs e)
        {
            try
            {
                string detail = "";
                var ex = HttpContext.Current.Error;
                while (ex != null)
                {
                    _log.Error("Error in webapplication" + detail, ex);
                    ex = ex.InnerException;
                    detail = ": inner exception:";
                }
            }
            catch
            {
                // IGNORE THIS
            }
        }
    }
}
