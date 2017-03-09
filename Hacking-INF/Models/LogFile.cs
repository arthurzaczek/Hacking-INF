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
                if (Message.StartsWith("\t")) return "red";
                if (Message.Contains("[DEBUG]")) return "darkgray";
                if (Message.Contains("[WARN]")) return "#aa0";
                if (Message.Contains("[ERROR]")) return "red";
                return "black";
            }
        }
    }
}