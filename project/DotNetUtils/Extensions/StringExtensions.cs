using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNetUtils
{
    public static class StringFormatExtensions
    {
        /// <summary>
        /// Encrypt mask for string to hex
        /// </summary>
        private const byte Mask = 0xac;

        /// <summary>
        /// Regex pattern for phone number validation
        /// </summary>
        private const string RegexPhoneNumber = @"^([0-9\(\)\/\+ \-x]*)$";

        /// <summary>
        /// helper array for char value to ascii number string
        /// </summary>
        private static readonly char[] CharAsciiMap = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// helper array for ascii string to char value
        /// </summary>
        ///----------------------------------------------------------//48-57 '0'-'9' --------------//58-64 -------------//65-70 'A'-'Z'
        private static readonly byte[] AsciiStringMap = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15, };

        /// <summary>
        /// Convert string to a series of hex number string with a little encryption
        /// </summary>
        /// <param name="src">the source string</param>
        /// <param name="enableCompression">specify if it should apply compression while converting to string</param>
        /// <returns>Encrypted string in hex format</returns>
        public static string ToHexString(this string src, bool enableCompression = false)
        {
            if (src.IsNullOrWhiteSpace())
            {
                return String.Empty;
            }
            var srcData = enableCompression ? LzfHelper.Compress(src) : Encoding.UTF8.GetBytes(src);
            var data = new byte[srcData.Length << 1];
            for (var i = 0; i < srcData.Length; i++)
            {
                var t = (byte)(srcData[i] ^ Mask);
                data[i * 2] = (byte)CharAsciiMap[t >> 4];
                data[i * 2 + 1] = (byte)CharAsciiMap[t % 0x10];
            }
            return Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Decrypt for hex formated string encrypted by ToHexString 
        /// </summary>
        /// <param name="src">encrypted string</param>
        /// <param name="enableCompression">specify if it should apply decompression while converting from string</param>
        /// <returns>decrypted string</returns>
        public static string DecodeHexString(this string src, bool enableCompression = false)
        {
            if (src.IsNullOrWhiteSpace())
            {
                return String.Empty;
            }
            var data = new byte[src.Length >> 1];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(((AsciiStringMap[src[i << 1] - '0'] << 4) | (AsciiStringMap[src[(i << 1) + 1] - '0'])) ^ Mask);
            }
            var finalData = enableCompression ? LzfHelper.DecompressAsString(data) : Encoding.UTF8.GetString(data);
            return finalData;
        }


        /// <summary>
        /// Determine if specific <paramref name="input"/> is a valid US SSN
        /// </summary>
        /// <param name="input">A input expected to be in US SSN format</param>
        /// <returns>
        ///     <c>True</c> if <paramref name="input"/> is in US SSN format. <c>False</c> otherwise
        /// </returns>
        public static bool IsValidSsn(this string input)
        {
            // US SSN should format like xxx-xx-xxxx or xxxxxxxxx
            var pattern = @"^\d{3}-?\d{2}-?\d{4}$";
            return !String.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Determine if specific <paramref name="input"/> is a valid email
        /// </summary>
        /// <param name="input">A input expected to be in email format</param>
        /// <returns>
        ///     <c>True</c> if <paramref name="input"/> is in email format. <c>False</c> otherwise
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsValidEmail(this string input)
        {
            return !String.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, Constants.EmailPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Determine if specific <paramref name="input"/> is a valid FSET Business Name
        /// </summary>
        /// <param name="input">A input expected to be in FSET Business Name format</param>
        /// <returns>
        ///     <c>True</c> if <paramref name="input"/> is in FSET Business Name format. <c>False</c> otherwise
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsFSETBusinessName(this string input)
        {
            string pattern = @"^(\s{0})(( {0,1}[a-zA-Z0-9\(\)&'\-#,.])*)+(\s{0})$";
            return !String.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Determine if specific <paramref name="input"/> is a valid FSET Street Address
        /// </summary>
        /// <param name="input">A input expected to be in FSET Street Address format</param>
        /// <returns>
        ///     <c>True</c> if <paramref name="input"/> is in FSET Street Address format. <c>False</c> otherwise
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsFSETStreetAddress(this string input)
        {
            string pattern = @"^(\s{0})(( {0,1}[a-zA-Z0-9/\-,.])*)+(\s{0})$";
            return !String.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Get valid email address from input string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>A valid email address match the email format. <c>Null</c> if no match found</returns>
        public static string GetValidEmailAddress(this string input)
        {
            if (input.IsNullOrWhiteSpace())
            {
                return null;
            }
            var updatedInput = input.Replace('<', ' ').Replace('>', ' ');
            if (updatedInput.IsNullOrWhiteSpace())
            {
                return null;
            }
            var tokens = updatedInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (token.IsValidEmail())
                {
                    return token;
                }
            }
            return null;
        }

        /// <summary>
        /// Generate a xml format Id list from id collection
        /// </summary>
        /// <param name="ids">id collection</param>
        /// <returns>xml id list in format "<IdList><Item Id="1"/><Item Id="2"/></IdList>"</returns>
        [DebuggerStepThrough]
        public static string ToIntIdListXml(this IEnumerable<int> ids)
        {
            var element = ids.ToIntIdListXElement();
            return element != null ? element.ToString() : null;
        }

        /// <summary>
        /// Generate a xml format Id list from id collection
        /// </summary>
        /// <param name="ids">id collection</param>
        /// <returns>xml id list in format "<IdList><Item Id="1"/><Item Id="2"/></IdList>"</returns>
        [DebuggerStepThrough]
        public static string ToBigIntIdListXml(this IEnumerable<long> ids)
        {
            var element = ids.ToBigIntIdListXElement();
            return element != null ? element.ToString() : null;
        }

        /// <summary>
        /// Generate a xml format Id list from id collection
        /// </summary>
        /// <param name="ids">id collection</param>
        /// <returns>xml id list in format "<IdList><Item Id="1"/><Item Id="2"/></IdList>"</returns>
        [DebuggerStepThrough]
        public static XElement ToIntIdListXElement(this IEnumerable<int> ids)
        {
            return ids == null ? null : ToStringIdListXElement(from id in ids select id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Generate a xml format Id list from id collection
        /// </summary>
        /// <param name="ids">id collection</param>
        /// <returns>xml id list in format "<IdList><Item Id="1"/><Item Id="2"/></IdList>"</returns>
        public static XElement ToStringIdListXElement(this IEnumerable<string> ids)
        {
            return ids == null ? null : new XElement(Constants.IdListElementName, from id in ids select new XElement(Constants.IdListItemName, new XAttribute(Constants.IdListAttributeName, id)));
        }

        /// <summary>
        /// Generate a xml format Id list from id collection
        /// </summary>
        /// <param name="ids">id collection</param>
        /// <returns>xml id list in format "<IdList><Item Id="1"/><Item Id="2"/></IdList>"</returns>
        [DebuggerStepThrough]
        public static XElement ToBigIntIdListXElement(this IEnumerable<long> ids)
        {
            return ids == null ? null : ToStringIdListXElement(from id in ids select id.ToString());
        }

        /// <summary>
        /// Parse and int Id collection from the string which created by the method above
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<int> ExtractIntIdList(this string src)
        {
            if (src == null)
            {
                return null;
            }
            if (src.IsNullOrEmpty())
            {
                return Enumerable.Empty<int>();
            }
            var element = XElement.Parse(src);
            return element.ExtractIntIdList();
        }

        /// <summary>
        /// Parse and int Id collection from the XElement which created by the method above
        /// </summary>
        public static IEnumerable<int> ExtractIntIdList(this XElement src)
        {
            if (src == null)
            {
                return null;
            }
            src.VerifyElementName(Constants.IdListElementName);
            return from item in src.Elements(Constants.IdListItemName) select item.GetAttributeInt(Constants.IdListAttributeName);
        }

        /// <summary>
        /// Parse and big int Id collection from the string which created by the method above
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<long> ExtractBigIntIdList(this string src)
        {
            if (src == null)
            {
                return null;
            }
            if (src.IsNullOrEmpty())
            {
                return Enumerable.Empty<long>();
            }
            var element = XElement.Parse(src);
            return element.ExtractBigIntIdList();
        }

        /// <summary>
        /// Parse and long Id collection from the XElement which created by the method above
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<long> ExtractBigIntIdList(this XElement src)
        {
            if (src == null)
            {
                return null;
            }
            src.VerifyElementName(Constants.IdListElementName);
            return from item in src.Elements(Constants.IdListItemName) select item.GetAttributeInt64(Constants.IdListAttributeName);
        }

        /// <summary>
        /// Split by delimiter with checking source string
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<string> SafeSplit(this string src, char delimiter = ',', bool reserveEmpty = false)
        {
            if (reserveEmpty)
            {
                return src == null ? Enumerable.Empty<string>() : src.Split(new[] { delimiter }, StringSplitOptions.None);
            }
            return src.IsNullOrWhiteSpace() ? Enumerable.Empty<string>() : src.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Generate a comma delimited string from a collection of int ids
        /// </summary>
        /// <param name="src">the id collection</param>
        /// <returns>the formated comma delimited string</returns>
        [DebuggerStepThrough]
        public static string ToCommaDelimitedIntIdString(this IEnumerable<int> src)
        {
            if (null == src)
            {
                return string.Empty;
            }
            var collection = new CommaDelimitedStringCollection();
            collection.AddRange(src.Select(id => id.ToString()).ToArray());
            return collection.ToString();
        }

        /// <summary>
        /// Parse a comma delimited int id string to int id collection
        /// </summary>
        /// <param name="src">the formated comma delimited string</param>
        /// <param name="skipInvalid">Skip the entry if it is not valid</param>
        /// <param name="delimiter">Delimiter for the raw array </param>
        /// <returns>parsed id collection</returns>
        public static IEnumerable<int> ExtractIntIdCollection(this string src, bool skipInvalid = false, char delimiter = ',')
        {
            return string.IsNullOrEmpty(src) ? null : src.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue =>
                {
                    int value = default(int);
                    if (!Int32.TryParse(idValue, out value) && !skipInvalid)
                    {
                        throw new ArgumentException("Invalid cast for {0} to be an integer".FormatInvariantCulture(idValue));
                    }
                    return value;
                });
        }

        /// <summary>
        /// Parse a comma delimited int id string to int id collection
        /// </summary>
        /// <param name="src">the formated comma delimited string</param>
        /// <param name="skipInvalid">Skip the entry if it is not valid</param>
        /// <param name="nullValueIdentifier">identifier for identifying the null value</param>
        /// <param name="delimiter">Delimiter for the raw array </param>
        /// <returns>parsed id collection</returns>
        public static IEnumerable<int?> ExtractNullableIntIdCollection(this string src, bool skipInvalid = false, string nullValueIdentifier = "%NONE%", char delimiter = ',')
        {
            return string.IsNullOrEmpty(src) ? null : src.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue =>
                {
                    if (!string.IsNullOrEmpty(nullValueIdentifier) && idValue.Equals(nullValueIdentifier, StringComparison.OrdinalIgnoreCase))
                    {
                        return (int?)null;
                    }
                    int value = default(int);
                    if (!Int32.TryParse(idValue, out value) && !skipInvalid)
                    {
                        throw new ArgumentException("Invalid cast for {0} to be an integer".FormatInvariantCulture(idValue));
                    }
                    return value;
                });
        }

        /// <summary>
        /// Parse a comma delimited big int id string to big int id collection
        /// </summary>
        /// <param name="src">the formated comma delimited string</param>
        /// <param name="skipInvalid">Skip the entry if it is not valid</param>
        /// <returns>parsed id collection</returns>
        public static IEnumerable<long> ExtractBigIntIdCollection(this string src, bool skipInvalid = false)
        {
            return string.IsNullOrEmpty(src) ? null : src.Split(Constants.CommaDelimCharArray, StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue =>
                {
                    long value = default(long);
                    if (!Int64.TryParse(idValue, out value) && !skipInvalid)
                    {
                        throw new ArgumentException("Invalid cast for {0} to be an 64bit integer".FormatInvariantCulture(idValue));
                    }
                    return value;
                });
        }

        /// <summary>
        /// Generate plain string value from a collection
        /// </summary>
        /// <param name="src">the source collection</param>
        /// <param name="encrypt">Whether to encrypt the string field, encrypt will help prevent symbol conflict with delimiters</param>
        /// <returns>the formated plain string value of format "1;name1,2;name2"</returns>
        public static string ToCommaDelimString<T>(this IEnumerable<T> src, bool encrypt = true)
        {
            if (src == null)
            {
                return String.Empty;
            }
            if (!src.Any())
            {
                return GetEmptyIdNameDictString();
            }
            var collection = new CommaDelimitedStringCollection();
            collection.AddRange(src.Select(node => encrypt ? node.ToString().ToHexString() : node.ToString()).ToArray());
            return collection.ToString();
        }

        /// <summary>
        /// Construct a collection from saved string by "ToCommaDelimString"
        /// </summary>
        /// <param name="value">the stored ids string value without encryption</param>
        /// <param name="parser">Logic for parsing string to T </param>
        /// <param name="encrypted">Tell the method whether there was encryption in the string field</param>
        /// <returns>the dictionary</returns>
        public static IEnumerable<T> ExtractFromCommaDelimString<T>(this string value, Func<string, T> parser, bool encrypted = true)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }
            if (IsEmptyIdNameCollection(value))
            {
                return Enumerable.Empty<T>();
            }
            return (from str in value.Split(Constants.CommaDelimCharArray, StringSplitOptions.RemoveEmptyEntries)
                    select parser(encrypted ? str.DecodeHexString() : str));
        }

        /// <summary>
        /// Generate plain string value from id name pair collection
        /// </summary>
        /// <param name="src">the source collection</param>
        /// <param name="encrypt">Whether to encrypt the string field, encrypt will help prevent symbol conflict with delimiters</param>
        /// <returns>the formated plain string value of format "1;name1,2;name2"</returns>
        public static string ToCommaDelimIdNameString(this IEnumerable<KeyValuePair<int, string>> src, bool encrypt = true)
        {
            if (src == null)
            {
                return String.Empty;
            }
            if (!src.Any())
            {
                return GetEmptyIdNameDictString();
            }
            var collection = new CommaDelimitedStringCollection();
            collection.AddRange(src.Select(node => "{0}{1}{2}".FormatInvariantCulture(node.Key, Constants.SemiDelim, encrypt ? node.Value.ToHexString() : node.Value)).ToArray());
            return collection.ToString();
        }

        /// <summary>
        /// Generate plain string value from id name pair collection
        /// </summary>
        /// <param name="src">the source collection</param>
        /// <param name="encrypt">Whether to encrypt the string field, encrypt will help prevent symbol conflict with delimiters</param>
        /// <returns>the formated plain string value of format "1;name1,2;name2"</returns>
        public static string ToCommaDelimIdNameString(this IEnumerable<KeyValuePair<long, string>> src, bool encrypt = true)
        {
            if (src == null)
            {
                return String.Empty;
            }
            if (!src.Any())
            {
                return GetEmptyIdNameDictString();
            }
            var collection = new CommaDelimitedStringCollection();
            collection.AddRange(src.Select(node => "{0}{1}{2}".FormatInvariantCulture(node.Key, Constants.SemiDelim, encrypt ? node.Value.ToHexString() : node.Value)).ToArray());
            return collection.ToString();
        }

        /// <summary>
        /// Construct Id name dictionary from saved id name string by "GetCommaDelimitedIdNamePairString"
        /// </summary>
        /// <param name="value">the stored ids string value without encryption</param>
        /// <param name="encrypted">Tell the method whether there was encryption in the string field</param>
        /// <returns>the dictionary</returns>
        public static IDictionary<int, string> ExtractIdNameDictionary(this string value, bool encrypted = true)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }
            if (IsEmptyIdNameCollection(value))
            {
                return new Dictionary<int, string>();
            }
            return value.ExtractIdNameCollection(encrypted).ToDictionary(i => i.Key, i => i.Value);
        }

        /// <summary>
        /// Construct Id name collection from saved id name string by "GetCommaDelimitedIdNamePairString"
        /// </summary>
        /// <param name="value">the stored ids string value without encryption</param>
        /// <param name="encrypted">Tell the method whether there was encryption in the string field</param>
        /// <returns>the dictionary</returns>
        public static IEnumerable<KeyValuePair<int, string>> ExtractIdNameCollection(this string value, bool encrypted = true)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }
            if (IsEmptyIdNameCollection(value))
            {
                return new Dictionary<int, string>();
            }
            return (from items in value.Split(Constants.CommaDelimCharArray, StringSplitOptions.RemoveEmptyEntries)
                    let fields = items.Split(Constants.SemiDelimCharArray, StringSplitOptions.RemoveEmptyEntries)
                    select new KeyValuePair<int, string>(Convert.ToInt32(fields[0]), encrypted ? fields[1].DecodeHexString() : fields[1]));
        }

        /// <summary>
        /// Construct rule Id name dictionary from saved id name string by "GetCommaDelimitedIdNamePairString"
        /// </summary>
        /// <param name="value">the stored ids string value without encryption</param>
        /// <param name="encrypted">Tell the method whether there was encryption in the string field</param>
        /// <returns>the dictionary</returns>
        public static IDictionary<long, string> ExtractBigIntIdNameDictionary(this string value, bool encrypted = true)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }
            if (IsEmptyIdNameCollection(value))
            {
                return new Dictionary<long, string>();
            }
            return value.ExtractBigIntIdNameCollection(encrypted).ToDictionary(i => i.Key, i => i.Value);
        }

        /// <summary>
        /// Construct Id name collection from saved id name string by "GetCommaDelimitedIdNamePairString"
        /// </summary>
        /// <param name="value">the stored ids string value without encryption</param>
        /// <param name="encrypted">Tell the method whether there was encryption in the string field</param>
        /// <returns>the dictionary</returns>
        public static IEnumerable<KeyValuePair<long, string>> ExtractBigIntIdNameCollection(this string value, bool encrypted = true)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }
            if (IsEmptyIdNameCollection(value))
            {
                return new Dictionary<long, string>();
            }
            return (from items in value.Split(Constants.CommaDelimCharArray, StringSplitOptions.RemoveEmptyEntries)
                    let fields = items.Split(Constants.SemiDelimCharArray, StringSplitOptions.RemoveEmptyEntries)
                    select new KeyValuePair<long, string>(Convert.ToInt64(fields[0]), encrypted ? fields[1].DecodeHexString() : fields[1]));
        }

        /// <summary>
        /// return the string value indicating the empty collection for ToCommaDelimIdNameString
        /// </summary>
        /// <returns>the empty collection indicator</returns>
        [DebuggerStepThrough]
        public static string GetEmptyIdNameDictString()
        {
            return Constants.SemiDelim;
        }

        /// <summary>
        /// To see if the string is indicating empty id name collection 
        /// </summary>
        /// <param name="value">the formated ids string</param>
        /// <returns>bool</returns>
        [DebuggerStepThrough]
        public static bool IsEmptyIdNameCollection(this string value)
        {
            return value != null && value.Equals(Constants.SemiDelim, StringComparison.Ordinal);
        }

        /// <summary>
        /// To see if a string is null or empty string
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>bool</returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string src)
        {
            return String.IsNullOrEmpty(src);
        }

        /// <summary>
        /// wrapper of string.format
        /// </summary>
        [DebuggerStepThrough]
        public static string Format(this string src, params object[] values)
        {
            return String.Format(CultureInfo.InvariantCulture, src, values);
        }

        /// <summary>
        /// wrapper of string.format
        /// </summary>
        [DebuggerStepThrough]
        public static string FormatInvariantCulture(this string src, params object[] values)
        {
            return String.Format(src, values);
        }

        /// <summary>
        /// To see if a string is null or white space
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>bool</returns>
        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string src)
        {
            return String.IsNullOrWhiteSpace(src);
        }

        /// <summary>
        /// Wrapper of string.Trim
        /// </summary>
        /// <param name="src">The string</param>
        /// <returns>if string is not null or whitespace, return trimmed version; otherwise return null.</returns>
        [DebuggerStepThrough]
        public static string TrimIfNotNullOrWhiteSpace(this string src)
        {
            return !src.IsNullOrWhiteSpace() ? src.Trim() : null;
        }

        /// <summary>
        /// Wrapper of string.length
        /// </summary>
        /// <param name="src">the string</param>
        /// <param name="trim">set to false to count whitespace as well</param>
        /// <returns>if src is not null or whitespace only, then return the actual lengh; otherwise return 0</returns>
        public static int Length(this string src, bool trim = true)
        {
            if (!trim)
            {
                return src == null ? 0 : src.Length;
            }
            return !src.IsNullOrWhiteSpace() ? src.Trim().Length : 0;
        }

        /// <summary>
        /// Validate string.length
        /// </summary>
        /// <param name="src">the string</param>
        /// <param name="minLength">min length</param>
        /// <param name="maxLength">max length</param>
        /// <param name="isDecimal">if or not need to decide src is or not a decimal</param>
        /// <returns>if src is not null and string length is in range  , then return true; otherwise return false</returns>
        public static bool IsLengthRange(this string src, int maxLength, int minLength = 0, bool isDecimal = false)
        {
            if (!src.IsNullOrWhiteSpace())
            {
                int length = src.Length;
                decimal value;
                return length >= minLength && length <= maxLength && (!isDecimal || decimal.TryParse(src, out value));
            }
            return false;
        }

        /// <summary>
        /// Check if a string is null or white space after html decode
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>bool</returns>
        [DebuggerStepThrough]
        public static bool IsHtmlNullOrWhiteSpace(this string src)
        {
            return src.HtmlDecode().IsNullOrWhiteSpace();
        }

        /// <summary>
        /// Wrapper for HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>Decoded string</returns>
        [DebuggerStepThrough]
        public static string HtmlDecode(this string src)
        {
            return src.IsNullOrWhiteSpace() ? String.Empty : HttpUtility.HtmlDecode(src);
        }

        /// <summary>
        /// HttpUtility.HtmlDecode then trim
        /// </summary>
        /// <param name="src">the string</param>
        /// <param name="defaultValue">default value if input string is null or whitespace</param>
        /// <returns>Decoded string</returns>
        [DebuggerStepThrough]
        public static string HtmlDecodeWithTrim(this string src, string defaultValue = "")
        {
            return src.IsNullOrWhiteSpace() ? defaultValue : HttpUtility.HtmlDecode(src).Trim();
        }

 /// <summary>
        /// HttpUtility.UrlDecode
        /// </summary>
        /// <param name="src">the string</param>
        /// <returns>Url decoded string</returns>
        [DebuggerStepThrough]
        public static string UrlDecodeWithTrim(this string src)
        {
            return src.IsNullOrWhiteSpace() ? String.Empty : HttpUtility.UrlDecode(src).Trim();
        }

        /// <summary>
        /// Parse the string to an Enum value if it is
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="src">the string</param>
        /// <returns>The enum value</returns>
        [DebuggerStepThrough]
        public static T ParseToEnum<T>(this string src) where T : struct
        {
            if (src.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(src));
            }
            return Util.ParseEnumValue<T, string>(src);
        }

        /// <summary>
        /// Parse the string to an Enum value if it is
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="src">the string</param>
        /// <returns>The enum value, return null if input is null or empty</returns>
        [DebuggerStepThrough]
        public static T? ParseToEnumNullable<T>(this string src) where T : struct
        {
            if (src.IsNullOrEmpty())
            {
                return null;
            }
            return Util.ParseEnumValue<T, string>(src);
        }

        /// <summary>
        /// Returns String.Empty if the input is null.
        /// </summary>
        /// <param name="str">The input string.</param>
        [DebuggerStepThrough]
        public static string EmptyIfNull(this string str)
        {
            return str ?? String.Empty;
        }

        /// <summary>
        /// Removes specific characters from current string
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="chars">Characters to be removed</param>
        public static string RemoveCharacter(this string str, char[] chars)
        {
            return chars.Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        /// <summary>
        /// Extracts out pure number characters from current string
        /// </summary>
        /// <param name="str">The input string.</param>
        public static string ExtractPureNumber(this string str)
        {
            return str.IsNullOrWhiteSpace() ? string.Empty : new string(str.Where(c => c >= '0' && c <= '9').ToArray());
        }

        /// <summary>
        /// Output fset format business name
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns></returns>
        public static string ToFSETBusinessName(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return str;
            }
            var result = Regex.Replace(str, @"[^a-zA-Z0-9\(\)&'\-# ]", "");
            result = Regex.Replace(result, @" {2,}", " ");
            return result;
        }

        /// <summary>
        /// Output fset format street address
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns></returns>
        public static string ToFSETStreetAddress(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return str;
            }
            var result = Regex.Replace(str, @"[^a-zA-Z0-9/\- ]", "");
            result = Regex.Replace(result, @" {2,}", " ");
            return result;
        }

        /// <summary>
        /// Validates if specific string is a valid Us phone number
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="acceptEmpty">Set to TRUE if should regard empty string as valid</param>
        public static bool IsValidUsPhoneNumber(this string str, bool acceptEmpty = false)
        {
            var reg = new Regex(RegexPhoneNumber);
            // set to true if accept empty
            if (str.IsNullOrEmpty() && acceptEmpty)
            {
                return true;
            }
            // validate by regular expression
            if (str.IsNullOrEmpty() || !reg.IsMatch(str))
            {
                return false;
            }
            // get pure number
            str = str.ExtractPureNumber();
            // validate number
            return !str.IsNullOrEmpty() && str.Length <= 20;
        }

        /// <summary>
        /// Validates if specified string is a pure number
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>Returns TRUE if the string is not null and all of its character in this string is number</returns>
        public static bool IsPureNumber(this string str)
        {
            return !str.IsNullOrEmpty() && str.All(Char.IsDigit);
        }

        public static string ToCurrencyString(this decimal number, string format)
        {
            return number > 0 ? number.ToString(format)
                : "({0})".FormatInvariantCulture((-number).ToString(format));
        }

        public static string SafeToString<T>(this T? val, string defaultVal) where T : struct
        {
            if (val.HasValue)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}", val.Value);
            }

            return defaultVal;
        }

        public static string SafeToString(this object val, string defaultVal = null)
        {
            var val1 = val as decimal?;
            if(val1 != null)
            {
                return val1.Value.ToStringTrim();
            }
            return val == null ? defaultVal ?? String.Empty : Convert.ToString(val);
        }

        /// <summary>
        /// Indicate if given path ends with any of specific file extensions or not
        /// </summary>
        /// <param name="relativePath">The relative path to be checked upon</param>
        /// <param name="fileExtensions">A collection of file extensions to be checked against</param>
        /// <param name="comparison">One of the enumeration values that determines how this string and value are compared.</param>
        /// <returns>True if <paramref name="relativePath"/> ends with any of extensions in <paramref name="fileExtensions"/>. Otherwise false.</returns>
        public static bool RelativeUriEndsWith(this string relativePath, string[] fileExtensions, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (relativePath.IsNullOrWhiteSpace() || !Uri.IsWellFormedUriString(relativePath, UriKind.Relative) || fileExtensions.IsNullOrEmpty())
            {
                return false;
            }
            return fileExtensions.Any(extension => relativePath.EndsWith(extension.Trim(), comparison));
        }

        /// <summary>
        /// Validates specified string hash code
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="hashCode">hash code</param>
        /// <param name="lowerCase">Only match lower case string hash code</param>
        /// <returns>Returns TRUE if the string hash code equal hash code value</returns>
        public static bool IsVaildHashCode(this string str, int hashCode, bool lowerCase = true)
        {
            return lowerCase ? hashCode.Equals(str.ToLower().GetHashCode()) : hashCode.Equals(str.GetHashCode());
        }

        /// <summary>
        /// Append line to string builder with null check
        /// </summary>
        /// <param name="builder">string builder</param>
        /// <param name="line">the line to append</param>
        public static void SafeAppendLine(this StringBuilder builder, string line)
        {
            if (builder != null && !line.IsNullOrWhiteSpace())
            {
                builder.AppendLine(line);
            }
        }

        /// <summary>
        /// Shorten input string if its length exceeds length limit
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="maxLength">Length limit</param>
        /// <param name="postfix">Postfix which will be appended to input string if it exceeds max length</param>
        /// <returns>The output string</returns>
        public static string Shorten(this string str, int maxLength, string postfix = "...")
        {
            if (str.IsNullOrWhiteSpace() || str.Length <= maxLength)
            {
                return str;
            }
            return (postfix.IsNullOrWhiteSpace() || postfix.Length >= maxLength)
                ? str.Substring(0, maxLength)
                : "{0}{1}".FormatInvariantCulture(str.Substring(0, maxLength - postfix.Length), postfix);
        }

        /// <summary>
        /// Parse string value to be a nullable instance
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="val">The value to be parsed</param>
        /// <returns>
        /// A nullable T instance
        /// </returns>
        public static T? ParseNullableValue<T>(this string val) where T : struct
        {
            if (val.IsNullOrWhiteSpace())
            {
                return null;
            }
            if (val.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            try
            {
                var tc = TypeDescriptor.GetConverter(typeof(T));
                return (T?)tc.ConvertFromString(val);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the string ends with number
        /// </summary>
        /// <param name="val">input string</param>
        /// <returns>True if it ends with number, otherwise returns false</returns>
        public static bool EndsWithNumber(this string val)
        {
            return !val.IsNullOrWhiteSpace() && Char.IsDigit(val, val.Length - 1);
        }

        /// <summary>
        /// Extracts ending number from the given string
        /// </summary>
        /// <param name="val">input string</param>
        /// <returns>number string</returns>
        public static string ExtractEndingNumber(this string val)
        {
            if (!val.EndsWithNumber())
            {
                return string.Empty;
            }
            var strBuilder = new StringBuilder();
            for (var i = val.Length - 1; i >= 0 && Char.IsDigit(val, i); i--)
            {
                strBuilder.Insert(0, val[i]);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Join a list of string to a single string separated by separator
        /// </summary>
        /// <param name="collection">the list of string</param>
        /// <param name="seperator">the separator between each 2 items</param>
        /// <returns>a single joined string</returns>
        public static string Join(this IEnumerable<string> collection, string seperator)
        {
            return String.Join(seperator, collection);
        }

        /// <summary>
        /// To lower with invariant culture setting with null checking
        /// </summary>
        public static string SafeToLowerInvariant(this string val, string defaultValue = null)
        {
            return val.IsNullOrEmpty() ? defaultValue : val.ToLowerInvariant();
        }

        /// <summary>
        /// Convert the original string to be fix length, appending specific char at the end if necessary
        /// </summary>
        /// <param name="original">The original string</param>
        /// <param name="targetLength">The target length</param>
        /// <param name="appendChar">The character to be appended if original string length than the target length</param>
        /// <param name="appendAfter">A value indicate append after or insert before</param>
        /// <param name="throwExceptionIfOverLength">A value to indicate if an exception will be thrown if original string length greater than target length</param>
        /// <returns>
        ///     A string with fixed length same as target length. 
        ///     If original string longer than target length, return a substring if <paramref name="throwExceptionIfOverLength"/> is false. Otherwise, throw exception
        /// </returns>
        public static string ToFixLength(this string original, int targetLength, char appendChar = ' ', bool appendAfter = true, bool throwExceptionIfOverLength = false)
        {
            if (targetLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetLength), "targetLength must be greater than 0");
            }
            if (original == null || original.Length == targetLength)
            {
                return original;
            }
            if (original.Length > targetLength)
            {
                if (throwExceptionIfOverLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(targetLength), "The targetLength is less than original string's length");
                }
                else
                {
                    return original.Substring(0, targetLength);
                }
            }
            var builder = new StringBuilder(original, targetLength);
            var diff = targetLength - original.Length;
            while (diff > 0)
            {
                if (appendAfter)
                {
                    builder.Append(appendChar);
                }
                else
                {
                    builder.Insert(0, appendChar);
                }
                diff--;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Right justified the original string to be fix length, fill the append char on the left side
        /// </summary>
        /// <param name="original">The original string</param>
        /// <param name="targetLength">The target length</param>
        /// <param name="appendChar">The character to be appended if original string length than the target length</param>
        /// <param name="throwExceptionIfOverLength">A value to indicate if an exception will be thrown if original string length greater than target length</param>
        /// <returns>
        ///     A string with fixed length same as target length and right justified 
        ///     If original string longer than target length, return a substring if <paramref name="throwExceptionIfOverLength"/> is false. Otherwise, throw exception
        /// </returns>
        public static string RightJustifyToFixLength(this string original, int targetLength, char appendChar = ' ', bool throwExceptionIfOverLength = false)
        {
            return original.ToFixLength(targetLength, appendChar, appendAfter: false, throwExceptionIfOverLength: throwExceptionIfOverLength);
        }

        /// <summary>
        /// Parse comma delimited id list to tagged xelement id list
        /// </summary>
        public static XElement FromIntCommaDelimListToXElementList(this string src, string tagName)
        {
            if (src.IsNullOrWhiteSpace() || tagName.IsNullOrWhiteSpace())
            {
                return null;
            }
            var ids = src.SafeSplit();
            var parsedId = new List<int>();
            foreach (var id in ids)
            {
                int val;
                // ignore invalid values
                if (!int.TryParse(id, out val))
                {
                    continue;
                }
                parsedId.Add(val);
            }
            var listElement = parsedId.ToIntIdListXElement();
            var taggedElement = new XElement(tagName, listElement);
            return taggedElement;
        }

        public static bool ContainsIgnoreCase(this string src, string pat)
        {
            if (src.IsNullOrEmpty() || pat.IsNullOrEmpty())
            {
                return false;
            }
            return src.IndexOf(pat, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static XElement ConvertToXElement(this string content, XNamespace ns)
        {
            if (content.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (ns == null)
            {
                throw new ArgumentNullException(nameof(ns));
            }
            var element = XElement.Parse(content);
            if (element == null)
            {
                throw new ArgumentException("content is not a valid xml element", nameof(content));
            }
            if (element.GetDefaultNamespace() == ns)
            {
                element.Attribute("xmlns").Remove();
            }
            return element;
        }

        public static string SafePadLeft(this string raw, int totalWidth, char paddingChar)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return "".PadLeft(totalWidth, paddingChar);
            }

            if (raw.Length > totalWidth)
            {
                return raw.Substring(raw.Length - totalWidth);
            }

            return raw.PadLeft(totalWidth, paddingChar);
        }

        public static string SafePadRight(this string raw, int totalWidth, char paddingChar)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return "".PadRight(totalWidth, paddingChar);
            }

            if (raw.Length > totalWidth)
            {
                return raw.Substring(0, totalWidth);
            }

            return raw.PadRight(totalWidth, paddingChar);
        }

        public static string SafeGetAt(this string raw, int position)
        {
            if (string.IsNullOrEmpty(raw) || position < 0 || position >= raw.Length)
            {
                return string.Empty;
            }

            return raw.Substring(position, 1);
        }

        private static readonly string[] Ones = {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", 
            "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
        };
        private static readonly string[] Tens = { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

        private static readonly string[] Thous = { "hundred", "thousand", "million", "billion", "trillion", "quadrillion" };

        public static string ToWords(this decimal number)
        {
            if (number < 0)
            {
                return "minus " + ToWords(Math.Abs(number));
            }

            var intPortion = (int)number;
            var decPortion = (int)((number - intPortion) * (decimal)100);

            return string.Format("{0} dollars and {1} cents", ToWords(intPortion), ToWords(decPortion));
        }

        private static string ToWords(int number, string appendScale = "")
        {
            var numString = "";
            if (number < 100)
            {
                if (number < 20)
                {
                    numString = Ones[number];
                }
                else
                {
                    numString = Tens[number / 10];
                    if ((number % 10) > 0)
                    {
                        numString += "-" + Ones[number % 10];
                    }
                }
            }
            else
            {
                var pow = 0;
                var powStr = "";

                if (number < 1000) // number is between 100 and 1000
                {
                    pow = 100;
                    powStr = Thous[0];
                }
                else // find the scale of the number
                {
                    var log = (int)Math.Log(number, 1000);
                    pow = (int)Math.Pow(1000, log);
                    powStr = Thous[log];
                }

                numString = string.Format("{0} {1}", ToWords(number / pow, powStr), ToWords(number % pow)).Trim();
            }
            return string.Format("{0} {1}", numString, appendScale).Trim();
        }

        public static StringBuilder Append(this StringBuilder builder, params object[] items)
        {
            if (builder == null)
            {
                return builder;
            }

            if (items.IsNullOrEmpty())
            {
                return builder;
            }

            foreach (var item in items)
            {
                builder.Append(item);
            }

            return builder;
        }

        public static void AddString(this IList<string> list, string pattern, params object[] args)
        {
            if (list != null)
            {
                list.Add(pattern.FormatInvariantCulture(args));
            }
        }
        public static IEnumerable<byte> GetUntil(this BinaryReader src, string pattern)
        {
            if (src == null || String.IsNullOrEmpty(pattern))
            {
                yield break;
            }
            var matching = 0;
            while (src.BaseStream.Position < src.BaseStream.Length)
            {
                var c = src.ReadByte();
                if (c == pattern[matching])
                {
                    if (matching < pattern.Length - 1)
                    {
                        matching++;
                    }
                    else
                    {
                        yield break;
                    }
                }
                else
                {
                    for (var i = 0; i < matching; i++)
                    {
                        yield return (byte)pattern[i];
                    }
                    matching = 0;
                    yield return (byte)c;
                }
            }
        }

        public static string GetUntil(this StreamReader src, string pattern)
        {
            if (src == null || String.IsNullOrEmpty(pattern))
            {
                return null;
            }
            var sb = new StringBuilder();
            int c;
            var matching = 0;
            while ((c = src.Read()) != -1)
            {
                sb.Append((char)c);
                if (c == pattern[matching])
                {
                    if (matching < pattern.Length - 1)
                    {
                        matching++;
                    }
                    else
                    {
                        var ret = sb.ToString();
                        if (ret.Length == pattern.Length)
                        {
                            return String.Empty;
                        }
                        return ret.Substring(0, ret.Length - pattern.Length);
                    }
                }
                else
                {
                    matching = 0;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get the sub string before the pattern's position
        /// </summary>
        /// <returns></returns>
        public static string GetUntil(this string src, string pattern, out string left, bool includePattern = false)
        {
            left = src;
            if (String.IsNullOrEmpty(src))
            {
                return src;
            }
            if (String.IsNullOrEmpty(pattern))
            {
                return src;
            }
            var matching = 0;
            var index = 0;
            while (index < src.Length && matching < pattern.Length)
            {
                var c = src[index];
                if (c == pattern[matching])
                {
                    if (matching < pattern.Length)
                    {
                        matching++;
                    }
                }
                else
                {
                    matching = 0;
                }
                index++;
            }
            index -= matching;
            left = index + 1 < src.Length ? src.Substring(index + (includePattern ? 0 : pattern.Length)) : String.Empty;
            return index > src.Length - 1 ? src : index < 1 ? String.Empty : src.Substring(0, index);
        }

    }
}
