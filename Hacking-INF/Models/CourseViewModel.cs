using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class CourseViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string HeadLine { get; set; }
        public string Description { get; set; }

        public CourseViewModel()
        {

        }

        public CourseViewModel(Course obj)
        {
            Refresh(obj);
        }

        public void Refresh(Course obj)
        {
            var target = this;
            var source = obj;

            target.Name = source.Name;
            target.Title = source.Title;
            target.HeadLine = source.HeadLine;
            target.Description = source.Description;
        }
    }
}