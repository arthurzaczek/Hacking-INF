using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class Course
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        [YamlMember(Alias = "title")]
        public string Title { get; set; }
        [YamlMember(Alias = "headline")]
        public string HeadLine { get; set; }
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
        [YamlMember(Alias = "exe")]
        public string Exe { get; set; }
        [YamlMember(Alias = "filename")]
        public string FileName { get; set; }
        [YamlMember(Alias = "compiler")]
        public List<Compiler> Compiler { get; set; }
    }

    public class Compiler
    {
        [YamlMember(Alias = "cmd")]
        public string Cmd { get; set; }
        [YamlMember(Alias = "args")]
        public string Args { get; set; }
        [YamlMember(Alias = "log")]
        public string Log { get; set; }
    }

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