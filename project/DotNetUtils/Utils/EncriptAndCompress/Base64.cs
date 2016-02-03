using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetUtils
{
    /// <summary>
    /// Base64 (UTF-8) encode and decode
    /// </summary>
    public static class Base64
    {
        /// <summary>
        /// decode
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns>utf-8 string</returns>
        public static string Decode(string base64String)
        {
            return !base64String.IsNullOrWhiteSpace() ? Encoding.UTF8.GetString(Convert.FromBase64String(base64String)) : string.Empty;
        }

        /// <summary>
        /// encode
        /// </summary>
        /// <param name="utf8String"></param>
        /// <returns>base64 string</returns>
        public static string Encode(string utf8String)
        {
            return !utf8String.IsNullOrWhiteSpace() ? Convert.ToBase64String(Encoding.UTF8.GetBytes(utf8String)) : string.Empty;
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
                throw new ArgumentException("Source type does not support Serialize-able attribute");
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
        /// De-serialize a Base64 encoded string to be an object instance
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="encodedString">The base64 encoded string</param>
        /// <returns>An object de-serialized from a Base64 string</returns>
        public static T DeserializeObjectFromBased64String<T>(string encodedString) where T : class
        {
            if (!typeof(T).IsDefined(typeof(SerializableAttribute), true))
            {
                throw new ArgumentException("Source type does not support Serialize-able attribute");
            }
            if (encodedString.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(encodedString));
            }
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encodedString)))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms) as T;
            }
        }
    }
}
