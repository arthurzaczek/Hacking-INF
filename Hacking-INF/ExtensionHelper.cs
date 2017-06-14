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

        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }
    }
}