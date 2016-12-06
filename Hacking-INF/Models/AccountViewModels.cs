using System;
using System.Collections.Generic;

namespace Hacking_INF.Models
{
    // Models returned by AccountController actions.

    public class LoginViewModel
    {
        public string UID { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public string[] Roles { get; set; } = new string[] { };

        public override string ToString()
        {
            return string.Format("{0};{1}", UID, string.Join(",", Roles));
        }
    }
}
