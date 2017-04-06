using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class ExampleViewModel
    {
        public string Name { get; set; }
        public string Course { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public List<string> Requires { get; set; }
        public string SourceCode { get; set; }
        public string UseThisMain { get; set; }
        public string Instruction { get; set; }
        public Guid SessionID { get; set; }
        public DateTime StartTime { get; set; }
        public List<TestFileViewModel> TestFiles { get; set; } = new List<TestFileViewModel>();
        public ExampleResultViewModel Result { get; set; }

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
            target.Description = source.Description;
            target.Type = source.Type.ToString();
            target.Category = source.Category.ToValidName();
            target.Difficulty = source.Difficulty;
            target.Requires = source.Requires ?? new List<string>();
        }
    }
}