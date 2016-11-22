using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class Example
    {
        [YamlMember(Alias = "problemshorttitle")]
        public string Name { get; set; }
        [YamlIgnore]
        public string Course { get; set; }
        [YamlMember(Alias = "problemtitle")]
        public string Title { get; set; }
    }

    public class ExampleViewModel
    {
        public string Name { get; set; }
        public string Course { get; set; }
        public string Title { get; set; }

        public ExampleViewModel()
        {

        }

        public ExampleViewModel(Example obj)
        {
            Refresh(obj);
        }

        public void Refresh(Example obj)
        {
            var target = this;
            var source = obj;

            target.Name = source.Name;
            target.Course = source.Course;
            target.Title = source.Title;
        }
    }
}