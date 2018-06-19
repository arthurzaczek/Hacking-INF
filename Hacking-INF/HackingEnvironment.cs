using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hacking_INF
{
    public class HackingEnvironment
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(HackingEnvironment));
        private static HackingEnvironment _current;
        private static readonly object _lock = new object();
        public static HackingEnvironment Current
        {
            get
            {
                if (_current != null) return _current;
                lock(_lock)
                {
                    if (_current == null)
                        _current = new HackingEnvironment();
                }
                return _current;
            }
        }

        private bool _initialized = false;
        private HackingEnvironment()
        {

        }

        public void InitFromHostingEnvironment()
        {
            if (!_initialized)
            {
                lock (_lock)
                {
                    if (_initialized) return;

                    ExamplesDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Examples");
                    WorkingDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/WorkingDir");
                    ToolsDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Tools");
                    SubmissionsDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Submissions");
                    SettingsDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Settings");
                    DataDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");

                    if (new[] { ExamplesDir, WorkingDir, ToolsDir, SubmissionsDir, SettingsDir, DataDir }.Any(i => string.IsNullOrWhiteSpace(i)))
                        throw new InvalidOperationException("Unable to initialize HackingEnvironment, at least one path is null.");

                    _log.Info("Initialied HackingEnvironment:");
                    _log.Info($"    ExamplesDir = {ExamplesDir}");
                    _log.Info($"    WorkingDir = {WorkingDir}");
                    _log.Info($"    ToolsDir = {ToolsDir}");
                    _log.Info($"    SubmissionsDir = {SubmissionsDir}");
                    _log.Info($"    SettingsDir = {SettingsDir}");
                    _log.Info($"    DataDir = {DataDir}");

                    _initialized = true;
                }
            }
        }

        public string ExamplesDir { get; private set; }
        public string WorkingDir { get; private set; }
        public string ToolsDir { get; private set; }
        public string SubmissionsDir { get; private set; }
        public string SettingsDir { get; private set; }
        public string DataDir { get; private set; }

    }
}
