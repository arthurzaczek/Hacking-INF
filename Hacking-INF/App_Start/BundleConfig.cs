using System.Web;
using System.Web.Optimization;

namespace Hacking_INF
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/es-shim")
                .Include(
                    "~/node_modules/es5-shim/es5-shim.min.js",
                    "~/node_modules/es6-shim/es6-shim.min.js"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/angular")
                .Include(
                    "~/node_modules/core-js/client/shim.js",
                    "~/node_modules/zone.js/dist/zone.js",
                    "~/node_modules/reflect-metadata/Reflect.js",
                    "~/node_modules/systemjs/dist/system.src.js",
                    "~/systemjs.config.js"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/scripts")
                .Include(
                    "~/node_modules/marked/marked.min.js",
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/highlight.pack.js",
                    "~/Scripts/ace/ace.js",
                    "~/Scripts/site.js"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/extra-scripts")
                .Include(
                    "~/Scripts/ace/ext-language_tools.js"
                )
            );

            bundles.Add(new StyleBundle("~/bundles/css")
                .Include(
                    "~/Content/bootstrap.css",
                    "~/Content/highlight/zenburn.css",
                    "~/Content/Site.css"
                )
            );
        }
    }
}
