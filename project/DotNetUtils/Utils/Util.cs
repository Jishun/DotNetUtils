using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetUtils
{
    public static class Util
    {
        /// <summary>
        /// Construct a name value dictionary based on given enumeration type
        /// </summary>
        /// <param name="enumType">The enumeration type</param>
        /// <param name="showDescription">A flag indicates whether to use description attribute's (if defined) text to replace the actual text</param>
        /// <returns>A dictionary which enumeration item's description or string representation as key and value as value</returns>
        public static IDictionary<string, int> ConstructEnumValueDictionary(Type enumType, bool showDescription = true, params object[] skips)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            var values = Enum.GetValues(enumType);
            var nameDict = new Dictionary<string, int>(values.Length);
            foreach (var value in values)
            {
                if (skips.Any(v => v.ToString() == value.ToString()))
                {
                    continue;
                }
                Enum item = (Enum)Enum.ToObject(enumType, value);
                nameDict.Add(showDescription ? item.GetDescription() : item.ToString(), (int)value);
            }
            return nameDict;
        }

        /// <summary>
        /// Construct a name value dictionary based on given enumeration type
        /// </summary>
        /// <param name="enumType">The enumeration type</param>
        /// <param name="showDescription">A flag indicates whether to use description attribute's (if defined) text to replace the actual text</param>
        /// <returns>A dictionary which enumeration item's description or string representation as key and value as value</returns>
        public static IDictionary<string, string> ConstructEnumNameDictionary(Type enumType, bool showDescription = true)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            var values = Enum.GetValues(enumType);
            var nameDict = new Dictionary<string, string>(values.Length);
            foreach (var value in values)
            {
                var item = (Enum)Enum.ToObject(enumType, value);
                nameDict.Add(showDescription ? item.GetDescription() : item.ToString(), value.ToString());
            }
            return nameDict;
        }

        /// <summary>
        /// Parse a value to be a enum value
        /// </summary>
        /// <typeparam name="T">The target enum type</typeparam>
        /// <typeparam name="TU">The type of value to be passed in</typeparam>
        /// <param name="value">The passed in value</param>
        /// <returns>
        ///     The enum value corresponding to passed in value.
        /// </returns>
        public static T ParseEnumValue<T, TU>(TU value) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Argument 'T' is not a enum");
            }
            if (typeof(TU).IsClass && value == null)
            {
                throw new ArgumentNullException("value");
            }
            var retValue = default(T);
            if (!Enum.TryParse<T>(value: value.ToString(), ignoreCase: true, result: out retValue))
            {
                throw new ArgumentException("value {0} is not a valid enum value".FormatInvariantCulture(value.ToString()));
            }
            return retValue;
        }

        /// <summary>
        /// Generic method for building a List object
        /// </summary>
        public static IList<T> ConstructList<T>(params T[] items)
        {
            return items.SafeGet().ToList();
        }

    }
}
