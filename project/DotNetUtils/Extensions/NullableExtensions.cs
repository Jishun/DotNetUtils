using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class NullableExtensions
    {
        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able short value</returns>
        public static short? NullIfDefault(this short inputValue, short defaultValue = default(short), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : (short?)inputValue)
                       : null;
        }

        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able int value</returns>
        public static int? NullIfDefault(this int inputValue, int defaultValue = default(int), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : (int?)inputValue)
                       : null;
        }

        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able int value</returns>
        public static int? NullIfDefault(this int? inputValue, int defaultValue = default(int), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : inputValue)
                       : null;
        }

        /// <summary>
        /// return specified default value if input is null, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return default if overall condition is not satisfied</param>
        /// <returns>int value</returns>
        public static int DefaultIfNull(this int? inputValue, int defaultValue = default(int), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == null ? defaultValue : inputValue.Value)
                       : defaultValue;
        }

        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able long value</returns>
        public static long? NullIfDefault(this long inputValue, long defaultValue = default(long), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : (long?)inputValue)
                       : null;
        }

        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able long value</returns>
        public static long? NullIfDefault(this long? inputValue, long defaultValue = default(long), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : inputValue)
                       : null;
        }

        /// <summary>
        /// return specified default value if input is null, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return default if overall condition is not satisfied</param>
        /// <returns>long value</returns>
        public static long DefaultIfNull(this long? inputValue, long defaultValue = default(long), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == null ? defaultValue : inputValue.Value)
                       : defaultValue;
        }

        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able long value</returns>
        public static decimal? NullIfDefault(this decimal inputValue, decimal defaultValue = default(decimal), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : (decimal?)inputValue)
                       : null;
        }

        /// <summary>
        /// return null if input equals to specified default value, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return null if overall condition is not satisfied</param>
        /// <returns>null able long value</returns>
        public static decimal? NullIfDefault(this decimal? inputValue, decimal defaultValue = default(decimal), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue == defaultValue ? null : inputValue)
                       : null;
        }

        /// <summary>
        /// return specified default value if input is null, otherwise the original value
        /// </summary>
        /// <param name="inputValue">the input value</param>
        /// <param name="defaultValue">default value</param>
        /// <param name="overallCondition">return default if overall condition is not satisfied</param>
        /// <returns>long value</returns>
        public static decimal DefaultIfNull(this decimal? inputValue, decimal defaultValue = default(decimal), bool overallCondition = true)
        {
            return overallCondition
                       ? (inputValue ?? defaultValue)
                       : defaultValue;
        }

        public static int? ParseIntNullable(this object value)
        {
            if (value == null)
            {
                return null;
            }
            var tempInt = value as int?;
            if (tempInt != null)
            {
                return tempInt;
            }
            var tempString = value as string ?? value.ToString();
            var i = 0;
            if (int.TryParse(tempString, out i))
            {
                return i;
            }
            return null;
        }

        /// <summary>
        /// Convert an object to decimal?
        /// </summary>
        /// <param name="value">the object to parse</param>
        /// <param name="round">Round decimal values</param>
        /// <returns>nullable decimal</returns>
        public static decimal? ParseDecimalNullable(this object value, int? round = null)
        {
            if (value != null)
            {
                var tempDouble = value as double?;
                if (tempDouble != null)
                {
                    return (decimal?)tempDouble;
                }
                var tempDecmial = value as decimal?;
                if (tempDecmial != null)
                {
                    return tempDecmial;
                }
                var tempString = value as string;
                if (tempString == null)
                {
                    tempString = value.ToString();
                }
                if (tempString != null)
                {
                    var decimalOnlyString = new String(tempString.ToCharArray().Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
                    decimal parsedValue;
                    if (Decimal.TryParse(decimalOnlyString, out parsedValue))
                    {
                        return round.HasValue ? decimal.Round(parsedValue, round.Value) : parsedValue;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Parse nullable instance to string
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="val">The value to be parsed</param>
        /// <returns>
        /// A string represents the nullable instance
        /// </returns>
        public static string NullableValueToString<T>(this T? val) where T : struct
        {
            if (!val.HasValue)
            {
                return "Null";
            }
            return val.Value.ToString();
        }

        public static decimal? ToNullableDecimal(this double? val)
        {
            return val.HasValue ? (decimal)val.Value : (decimal?)null;
        }

        public static double? ToNullableDouble(this decimal? val)
        {
            return val.HasValue ? (double)val.Value : (double?)null;
        }
    }
}
