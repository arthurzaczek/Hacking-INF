using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hacking_INF.Models
{
    public class LogLineModelViewModel
    {
        public string Message { get; set; }
        public string Color
        {
            get
            {
                if (Message.StartsWith("\t")) return "error";
                if (Message.Contains("[DEBUG]")) return "debug";
                if (Message.Contains("[WARN]")) return "warning";
                if (Message.Contains("[ERROR]")) return "error";
                return "info";
            }
        }
    }
}