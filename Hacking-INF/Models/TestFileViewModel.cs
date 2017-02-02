using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class TestFileViewModel
    {
        public string Name { get; set; }
        public string In { get; set; }
        public string SExp { get; set; }
    }
}