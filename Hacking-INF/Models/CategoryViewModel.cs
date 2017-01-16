using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class CategoryViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
 
        public CategoryViewModel()
        {

        }

        public CategoryViewModel(Category obj)
        {
            Refresh(obj);
        }

        public void Refresh(Category obj)
        {
            var target = this;
            var source = obj;

            target.Name = source.Name.ToValidName();
            target.Title = source.Title;
            target.Description = source.Description;
        }
    }
}