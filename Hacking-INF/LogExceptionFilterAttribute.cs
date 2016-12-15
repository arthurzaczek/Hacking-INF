using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;

namespace Hacking_INF
{
    public class LogExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            try
            {
                var log = log4net.LogManager.GetLogger(typeof(WebApiApplication));
                string detail = "";
                var ex = context.Exception;
                while (ex != null)
                {
                    log.Error("Error in webapplication" + detail, ex);
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