using Hacking_INF.Models;
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

        public static double? Median<TColl, TValue>(this IEnumerable<TColl> source, Func<TColl, TValue> selector)
        {
            return source.Select<TColl, TValue>(selector).Median();
        }

        public static double? Median<T>(this IEnumerable<T> source)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
                source = source.Where(x => x != null);

            int count = source.Count();
            if (count == 0)
                return null;

            source = source.OrderBy(n => n);

            int midpoint = count / 2;
            if (count % 2 == 0)
                return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
            else
                return Convert.ToDouble(source.ElementAt(midpoint));
        }

        public static IEnumerable<T> WhereStatus<T>(this IEnumerable<T> qry, bool isAuthenticated, bool isTeacher) where T : IStatus
        {
            var now = DateTime.Now;
            return qry.Where(i =>
            {
                if (isTeacher)
                {
                    return true;
                }

                if (i.ClosedFrom.HasValue
                 && i.ClosedUntil.HasValue
                 && i.ClosedFrom.Value <= now
                 && i.ClosedUntil.Value >= now)
                {
                    return false;
                }
                if (i.Type == Types.Open) return true;
                if (i.Type == Types.Closed) return false;
                if (i.Type == Types.Timed)
                {
                    if (!isAuthenticated) return false;
                    if (i.OpenFrom.HasValue
                     && i.OpenUntil.HasValue
                     && i.OpenFrom.Value <= now
                     && i.OpenUntil.Value >= now)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false; // Fail save. Show less, maybe we're missing a exam example                  
            });
        }

        public static IEnumerable<T> ReflectStatus<T>(this IEnumerable<T> qry, bool isTeacher) where T : IStatus
        {
            var now = DateTime.Now;
            return qry.Select(x =>
            {
                var i = (T)x.Clone();
                if (isTeacher)
                {
                    if (i.Type == Types.Timed)
                    {
                        // Reflect actual state
                        if (i.OpenFrom.HasValue
                         && i.OpenUntil.HasValue
                         && i.OpenFrom.Value <= now
                         && i.OpenUntil.Value >= now)
                        {
                            i.Type = Types.Timed;
                        }
                        else
                        {
                            i.Type = Types.Closed;
                        }
                    }
                    if (i.ClosedFrom.HasValue
                         && i.ClosedUntil.HasValue
                         && i.ClosedFrom.Value <= now
                         && i.ClosedUntil.Value >= now)
                    {
                        i.Type = Types.Closed;
                    }
                }

                return i;
            });
        }
    }
}