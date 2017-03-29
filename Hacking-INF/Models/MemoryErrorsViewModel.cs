using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class MemoryErrorsViewModel
    {
        public string TestCase { get; set; }
        public string Report { get; set; }

        public MemoryErrorsViewModel()
        {

        }

        public MemoryErrorsViewModel(MemoryErrors obj)
        {
            Refresh(obj);
        }

        public void Refresh(MemoryErrors obj)
        {
            var target = this;
            var source = obj;

            target.TestCase = source.TestCase;
            target.Report = source.Report;
        }
    }
}