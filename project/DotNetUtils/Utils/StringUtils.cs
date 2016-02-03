using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class StringUtils
    {
        public static string GetCommaDelimitedString(params object[] items)
        {
            if (items == null || items.Length < 1)
            {
                return string.Empty;
            }

            return string.Join(",", items);
        }
        /// <summary>
        /// Convert a list of string to one string
        /// </summary>
        /// <param name="list">The list to be converted</param>
        /// <returns>A string converted from the list</returns>
        public static string ConvertListToString(IEnumerable<string> list)
        {
            return string.Join(",", list);
        }

        public static string GenerateRandomString(int length)
        {
            var randomString = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                randomString.Append(Constants.LowerCharArray[(int)(Constants.LowerCharArray.Length * random.NextDouble())]);
            }
            return randomString.ToString();
        }
        /// <summary>
        /// Construct a currency format string
        /// </summary>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currency format string</returns>
        public static string GetCurrencyFormatSpecifier(string symbol)
        {
            return GetCurrencyFormatString(Constants.CurrencyFormatSpecifierPattern, symbol);
        }

        /// <summary>
        /// Construct a currency format string with currency symbol and decimal part
        /// </summary>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currency format string</returns>
        public static string GetCurrencyFormatWithDecimalSpecifier(string symbol)
        {
            return GetCurrencyFormatString(Constants.CurrencyFormatWithDecimalSpecifierPattern, symbol);
        }

        /// <summary>
        /// Construct a currency format string with currency symbol and decimal part up to 4 digits
        /// </summary>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currency format string</returns>
        public static string GetCurrencyFormatWith4DigitsDecimalSpecifier(string symbol)
        {
            return GetCurrencyFormatString(Constants.CurrencyFormatWith4DigitsDecimalSpecifierPattern, symbol);
        }

        /// <summary>
        /// Construct a currency format string
        /// </summary>
        /// <param name="pattern">The pattern string</param>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currency format string</returns>
        private static string GetCurrencyFormatString(string pattern, string symbol)
        {
            if (pattern.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("pattern");
            }
            if (symbol.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("symbol");
            }
            return pattern.FormatInvariantCulture(symbol);
        }

    }
}
