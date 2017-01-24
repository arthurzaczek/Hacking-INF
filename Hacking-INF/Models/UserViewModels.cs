using System;
using System.Linq;
using System.Collections.Generic;

namespace Hacking_INF.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {

        }

        public UserViewModel(User obj)
        {
            Refresh(obj);
        }

        public void Refresh(User obj)
        {
            var target = this;
            var source = obj;

            target.UID = obj.UID;
            target.Name = obj.Name;
        }

        public string UID { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }
        public string Jwt { get; set; }

        public string[] Roles { get; set; } = new string[] { };
        public bool IsTeacher
        {
            get
            {
                return Roles?.Any(i => i == "Teacher") ?? false;
            }
        }

        public IEnumerable<ExampleResultViewModel> Results { get; set; }

        public override string ToString()
        {
            return string.Format("{0};{1}", UID, string.Join(",", Roles));
        }
    }
}
