using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hacking_INF
{
    public static class ExtensionHelper
    {
        public static string ToValidName(this string str)
        {
            if (str == null) return null;

            return str
                .Replace(" ", "")
                .Replace("\t", "")
                .ToLower();
        }
    }
}