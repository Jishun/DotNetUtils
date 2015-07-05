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
    public static partial class Util
    {
        /// <summary>
        /// For Creating Salt and password Hash
        /// </summary>
        /// <returns>Salt String</returns>
        public static string CreateSalt(int bytes)
        {
            // Generate a cryptographic random number using the Cryptographic service provider
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[bytes];
            rng.GetBytes(buff);

            // Return Base64 representation of Random Number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        ///  Creates Password Hash
        /// </summary>
        /// <param name="passwordAndSalt">The password and salt.</param>
        /// <returns>Password Hash String</returns>
        public static string CreatePasswordHash(string passwordAndSalt)
        {
            //// Convert into GetBytes
            byte[] sourceStringToBytes = (new UnicodeEncoding()).GetBytes(passwordAndSalt);
            //// convert bytes using MD5CryptoServiceProvider
            byte[] hashedBytes = new MD5CryptoServiceProvider().ComputeHash(sourceStringToBytes);
            //// convert into Bit
            string hashedPasswd = BitConverter.ToString(hashedBytes);
            return hashedPasswd;
        }

        /// <summary>
        /// Creates Password Hash by PBKDF2
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="salt">salt</param>
        /// <returns>Password Hash String</returns>
        public static string CreatePasswordHash(string password, string salt)
        {
            byte[] hashedBytes = new Rfc2898DeriveBytes(password, (new UnicodeEncoding()).GetBytes(salt), PBKDF2IterationCount).GetBytes(PBKDF2Bytes);
            return BitConverter.ToString(hashedBytes); ;
        }

        /// <summary>
        /// Construct a full name string
        /// </summary>
        /// <param name="firstName">the first name</param>
        /// <param name="lastName">the last name</param>
        /// <param name="middleName">reserved for middle name, not contained in result yet</param>
        /// <returns>The full name string</returns>
        public static string ConstructFullName(string firstName, string lastName, string middleName = null)
        {
            return String.Concat(firstName ?? String.Empty, " ", lastName ?? String.Empty);
        }

        /// <summary>
        /// Translate a list of DayOfWeek value to a short integer value
        /// </summary>
        /// <param name="values">The list of DayOfWeek values</param>
        /// <returns>A short integer value represents the list</returns>
        public static short GenerateDaysOfWeekValue(IEnumerable<DayOfWeek> values)
        {
            short retFlag = 0x0000; //0x0000
            foreach (var value in values)
            {
                switch (value)
                {
                    case DayOfWeek.Monday:
                        retFlag |= 0x0001; //0000000000000001
                        break;
                    case DayOfWeek.Tuesday:
                        retFlag |= 0x0002; //0000000000000010
                        break;
                    case DayOfWeek.Wednesday:
                        retFlag |= 0x0004; //0000000000000100
                        break;
                    case DayOfWeek.Thursday:
                        retFlag |= 0x0008; //0000000000001000
                        break;
                    case DayOfWeek.Friday:
                        retFlag |= 0x0010; //0000000000010000
                        break;
                    case DayOfWeek.Saturday:
                        retFlag |= 0x0020; //0000000000100000
                        break;
                    case DayOfWeek.Sunday:
                        retFlag |= 0x0040; //0000000001000000
                        break;
                }
            }
            return retFlag;
        }

        /// <summary>
        /// Parse a short integer value to a list of DayOfWeek value
        /// </summary>
        /// <param name="value">The value to be parsed</param>
        /// <returns>A list of DayOfWeek value</returns>
        public static IList<DayOfWeek> ParseDaysOfWeekValue(short value)
        {
            IList<DayOfWeek> retList = new List<DayOfWeek>(7);
            if ((value & 0x0001) == 0x0001) //0000000000000001
            {
                retList.Add(DayOfWeek.Monday);
            }
            if ((value & 0x0002) == 0x0002)//0000000000000010
            {
                retList.Add(DayOfWeek.Tuesday);
            }
            if ((value & 0x0004) == 0x0004)//0000000000000100
            {
                retList.Add(DayOfWeek.Wednesday);
            }
            if ((value & 0x0008) == 0x0008)//0000000000001000
            {
                retList.Add(DayOfWeek.Thursday);
            }
            if ((value & 0x0010) == 0x0010)//0000000000010000
            {
                retList.Add(DayOfWeek.Friday);
            }
            if ((value & 0x0020) == 0x0020)//0000000000100000
            {
                retList.Add(DayOfWeek.Saturday);
            }
            if ((value & 0x0040) == 0x0040)//0000000001000000
            {
                retList.Add(DayOfWeek.Sunday);
            }
            return retList;
        }

        /// <summary>
        /// Translate a list of days of Month value to a string
        /// </summary>
        /// <param name="daysOfMonth">The list of values</param>
        /// <returns>A string representation of the values, in value1, value2, value3, ... format</returns>
        public static string GenerateDaysOfMonthString(IEnumerable<ushort> daysOfMonth)
        {
            CommaDelimitedStringCollection collection = new CommaDelimitedStringCollection();
            collection.AddRange(daysOfMonth.Select(day => day.ToString()).ToArray());
            return collection.ToString();
        }

        /// <summary>
        /// Parse a string value in format of value1, value2, value3,... to be a list of short integer represents the days in month
        /// </summary>
        /// <param name="values">The string representation</param>
        /// <returns>A list of short integer represents days in month</returns>
        public static IEnumerable<ushort> ParseDaysOfMonthValue(string values)
        {
            if (values.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("values");
            }
            return values.Split(Constants.CommaDelimCharArray, StringSplitOptions.RemoveEmptyEntries).Select(day => Convert.ToUInt16(day.Trim()));
        }

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
        /// Get a named connection string from Configuration
        /// </summary>
        /// <param name="name">The connection string name.</param>
        /// <returns>
        ///     The connection string if it exists in Configuration. Null otherwise
        /// </returns>
        public static string GetConnectionString(string name)
        {
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("name");
            }
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[name];
            if (setting == null)
            {
                throw new ArgumentException("Cannot load configuration setting with name '{0}'".FormatInvariantCulture(name));
            }
            return setting.ConnectionString;
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
        /// Count the number of weekday during <paramref name="startDate"/> and <paramref name="endDate"/>
        /// </summary>
        /// <param name="startDate">The startDate</param>
        /// <param name="endDate">The endDate</param>
        /// <param name="interval">The week interval</param>
        /// <returns>count</returns>
        public static int GetWeekdayCount(DateTime startDate, DateTime endDate, int interval = 1)
        {
            return GetDaysOfWeekCount(startDate, endDate, interval, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                      DayOfWeek.Thursday, DayOfWeek.Friday);
        }

        /// <summary>
        /// Calculate the count day in the given collection in specified range 
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate"> end date</param>
        /// <param name="interval">The week interval</param>
        /// <param name="daysOfWeek">the day collection</param>
        /// <returns>count</returns>
        public static int GetDaysOfWeekCount(DateTime startDate, DateTime endDate, int interval, params DayOfWeek[] daysOfWeek)
        {
            if (interval < 1)
            {
                throw new ArgumentOutOfRangeException("interval");
            }
            var total = (endDate.Date - startDate.Date).Days + 1;
            if (total <= 0)
            {
                return 0;
            }
            var weeks = total / (7 * interval);
            var remainder = total % (7 * interval);
            var newRemainder = remainder % 7;
            if (newRemainder == 0 && remainder > 0)
            {
                return (weeks + 1) * daysOfWeek.Count();
            }
            var days = 0;
            while (newRemainder > 0)
            {
                days += daysOfWeek.Any(d => d == startDate.DayOfWeek) ? 1 : 0;
                startDate = startDate.AddDays(1);
                newRemainder--;
            }
            return days + weeks * daysOfWeek.Count();
        }

        /// <summary>
        /// Calculate the least common multiplier
        /// </summary>
        /// <param name="a">number 1</param>
        /// <param name="b">numer 2</param>
        /// <returns>least common multiplier</returns>
        public static uint LCM(uint a, uint b)
        {
            return a * b / GCD(a, b);
        }

        /// <summary>
        /// calculate the Great common divisor  
        /// </summary>
        /// <param name="a">number 1</param>
        /// <param name="b">number 2</param>
        /// <returns>Great common divisor</returns>
        public static uint GCD(uint a, uint b)
        {
            uint r;
            if (a > b)
            {
                b = b ^ a;
                a = b ^ a;
                b = b ^ a;
            }
            while ((r = b % a) != 0)
            {
                b = a;
                a = r;
            }
            return r;
        }

        /// <summary>
        /// Load a file as xml
        /// </summary>
        /// <param name="filePath">The path of xml file</param>
        /// <returns>
        ///     XElement represents xml
        /// </returns>
        public static XElement LoadFileAsXml(string filePath)
        {
            if (filePath.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("filePath");
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentNullException("Cannot find file with path '{0}'".FormatInvariantCulture(filePath));
            }
            using (var reader = File.OpenText(filePath))
            {
                return XElement.Load(reader);
            }
        }

        /// <summary>
        /// Serialize an object to base 64 string
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized</typeparam>
        /// <param name="target">The object instance</param>
        /// <returns>A Base64 encoded string</returns>
        public static string SerializeObjectToBase64String<T>(T target) where T : class
        {
            if (!typeof(T).IsDefined(typeof(SerializableAttribute), true))
            {
                throw new ArgumentException("Source type does not support Serializable attribute");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, target);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserialize a Base64 encoded string to be an object instance
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="encodedString">The base64 encoded string</param>
        /// <returns>An object deserialized from a Base64 string</returns>
        public static T DeserializeObjectFromBased64String<T>(string encodedString) where T : class
        {
            if (!typeof(T).IsDefined(typeof(SerializableAttribute), true))
            {
                throw new ArgumentException("Source type does not support Serializable attribute");
            }
            if (encodedString.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("encodedString");
            }
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encodedString)))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms) as T;
            }
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

        /// <summary>
        /// Get magnitude of a decimal value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The magnitude of the value</returns>
        public static decimal GetMagnitude(decimal value)
        {
            var intValue = (int)Math.Abs(value);
            int count = 0;
            while (intValue / 10 != 0)
            {
                count++;
                intValue = intValue / 10;
            }
            decimal ret = (intValue + 1);
            while (count-- > 0)
            {
                ret = ret * 10;
            }
            return value > 0 ? ret : -ret;
        }

        public static string GenerateRandomString(int length)
        {
            var randomString = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                randomString.Append(Constants.LowerCharArray[(int)(Constants.LowerCharArray.Length * random.NextDouble())]);
            }
            return randomString.ToString();
        }

        /// <summary>
        /// AES Encrypt
        /// </summary>
        /// <param name="src">source string</param>
        /// <param name="password">password</param>
        /// <param name="iv">Encrypt key</param>
        /// <returns></returns>
        public static string AESEncrypt(string src, string iv, string password = null)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] keyBytes = new byte[16];
            if (!password.IsNullOrEmpty())
            {
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
                int len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
            }
            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(src);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            return Convert.ToBase64String(cipherBytes);

        }

        /// <summary>
        /// AES Decrypt
        /// </summary>
        /// <param name="src"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecrypt(string src, string iv, string password = null)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(src);

            byte[] keyBytes = new byte[16];
            if (!password.IsNullOrEmpty())
            {
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
                int len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
            }
            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// Generic method for building a List object
        /// </summary>
        public static IList<T> ConstructList<T>(params T[] items)
        {
            return items.SafeGet().ToList();
        }

        /// <summary>
        /// Construct a currency format string
        /// </summary>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currrency format string</returns>
        public static string GetCurrencyFormatSpecifier(string symbol)
        {
            return GetCurrencyFormatString(Constants.CurrencyFormatSpecifierPattern, symbol);
        }

        /// <summary>
        /// Construct a currency format string with currency symbol and decimal part
        /// </summary>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currrency format string</returns>
        public static string GetCurrencyFormatWithDecimalSpecifier(string symbol)
        {
            return GetCurrencyFormatString(Constants.CurrencyFormatWithDecimalSpecifierPattern, symbol);
        }

        /// <summary>
        /// Construct a currency format string with currency symbol and decimal part up to 4 digits
        /// </summary>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currrency format string</returns>
        public static string GetCurrencyFormatWith4DigitsDecimalSpecifier(string symbol)
        {
            return GetCurrencyFormatString(Constants.CurrencyFormatWith4DigitsDecimalSpecifierPattern, symbol);
        }

        /// <summary>
        /// Construct a currency format string
        /// </summary>
        /// <param name="pattern">The pattern string</param>
        /// <param name="symbol">The currency symbol</param>
        /// <returns>A currrency format string</returns>
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

        /// <summary>
        /// Function to get byte array from a file. 
        /// </summary>
        /// <param name="fileName">File name to get byte array</param>
        /// <returns>Byte Array</returns>
        public static byte[] FileToByteArray(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentNullException("Cannot find file with path '{0}'".FormatInvariantCulture(fileName));
            }
            byte[] buffer = null;
            BinaryReader binaryReader = null;
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    binaryReader = new BinaryReader(fileStream);
                    long totalBytes = new FileInfo(fileName).Length;
                    buffer = binaryReader.ReadBytes((Int32)totalBytes);
                    fileStream.Close();
                    fileStream.Dispose();
                }
                binaryReader.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Exception caught in process: {0}".FormatInvariantCulture(exception.ToString()));
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Dispose();
                }
            }
            return buffer;
        }

        public static string GetCommaDelimitedString(params object[] items)
        {
            if (items == null || items.Length < 1)
            {
                return string.Empty;
            }

            return string.Join(",", items);
        }
    }
}
