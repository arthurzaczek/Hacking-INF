using System.Web;
using System.Web.Optimization;

namespace Hacking_INF
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/libs/runtime*",
                      "~/Scripts/libs/polyfills*",
                      "~/Scripts/libs/vendor*",
                      "~/Scripts/libs/main*"));

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

            bundles.Add(new ScriptBundle("~/bundles/scripts-extra")
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
