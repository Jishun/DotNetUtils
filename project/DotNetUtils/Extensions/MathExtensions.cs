using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class MathExtensions
    {
        public static double RoundUp(this double val, double p)
        {
            var power = Math.Pow(10, p);
            return Math.Round(val / power + 0.5) * power;
        }

        public static T Min<T>(params T[] values)
        {
            return values.Min();
        }

        public static T Max<T>(params T[] values)
        {
            return values.Max();
        }

        public static T LimitRange<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            if (val.CompareTo(max) > 0)
            {
                return max;
            }
            return val;
        }

        public static decimal RoundToFixDecimal(this decimal original, int roundTo)
        {
            if (roundTo < 0)
            {
                return original;
            }
            return Decimal.Round(original, roundTo);
        }

        public static decimal NonNegative(this decimal original)
        {
            return original.CompareTo(0m) >= 0 ? original : 0m;
        }

        public static string CurrencyOmitDecimalAsString(this decimal original)
        {
            return (original * 100).ToString("0");
        }
    }
}
