using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value for the given key, or a default value if the key is not found.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of return value</typeparam>
        /// <param name="dictionary">The dictionary contains the data</param>
        /// <param name="key">The key of value to be retrieved</param>
        /// <param name="defaultValue">return specified default value if no match found</param>
        /// <returns>The value identified by the key in the collection. Or null if collection is null or no such key exists</returns>
        [DebuggerStepThrough]
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue result;
            if (dictionary.IsNullOrEmpty() || !dictionary.TryGetValue(key, out result))
            {
                return defaultValue;
            }

            return result;
        }

        /// <summary>
        /// add a key value pair into a dictionary, skip if the key already existed
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="dictionary">the dictionary</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>true if added, false if skipped</returns>
        [DebuggerStepThrough]
        public static bool AddOrSkip<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add each item in a collection to target dictionary, skip items if already existed in the dictionary
        /// </summary>
        /// <typeparam name="TKey">Key's type of the dictionary</typeparam>
        /// <typeparam name="TValue">Value's type of the dictionary</typeparam>
        /// <typeparam name="TSource">The source collection's item type</typeparam>
        /// <param name="dictionary">the target dictionary</param>
        /// <param name="collection">the source collection</param>
        /// <param name="keySelector">key selector</param>
        /// <param name="valueSelector">value selector</param>
        [DebuggerStepThrough]
        public static void AddRangeWithSkipExisted<TKey, TValue, TSource>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TSource> collection, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            foreach (var source in collection.EmptyIfNull())
            {
                dictionary.AddOrSkip(keySelector(source), valueSelector(source));
            }
        }

        /// <summary>
        /// Add each item in a collection to target dictionary, skip items if already existed in the dictionary
        /// </summary>
        /// <typeparam name="TKey">Key's type of the dictionary</typeparam>
        /// <typeparam name="TValue">Value's type of the dictionary</typeparam>
        /// <param name="dictionary">the target dictionary</param>
        /// <param name="collection">the source collection, item time should be TValue</param>
        /// <param name="keySelector">key selector</param>
        [DebuggerStepThrough]
        public static void AddRangeWithSkipExisted<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> collection, Func<TValue, TKey> keySelector)
        {
            foreach (var source in collection.EmptyIfNull())
            {
                dictionary.AddOrSkip(keySelector(source), source);
            }
        }

        /// <summary>
        /// add a key value pair into a dictionary, Overwrite the value if the key already existed
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="dictionary">the dictionary</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>true if added, false if Overwritten</returns>
        [DebuggerStepThrough]
        public static bool AddOrOverwrite<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            dictionary[key] = value;
            return false;
        }

        [DebuggerStepThrough]
        public static bool AddOrSkip<TKey>(this HashSet<TKey> hash, TKey key)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }
            if (!hash.Contains(key))
            {
                hash.Add(key);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Convert a collection to dictionary with the safe option skipDupe
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="TKey">key type of the dict</typeparam>
        /// <typeparam name="TValue">value type of the dict</typeparam>
        /// <param name="collection">the collection</param>
        /// <param name="keySelector">function to get key from item</param>
        /// <param name="valueSelector">function to get value from item</param>
        /// <param name="skipDupe">set to true if want to safely skip duplications</param>
        /// <returns>dictionary</returns>
        [DebuggerStepThrough]
        public static IDictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> collection, Func<T, TKey> keySelector, Func<T, TValue> valueSelector, bool skipDupe)
        {
            if (collection == null)
            {
                return null;
            }
            var ret = new Dictionary<TKey, TValue>();
            foreach (var item in collection)
            {
                var key = keySelector(item);
                if (skipDupe && ret.ContainsKey(key))
                {
                    continue;
                }
                ret.Add(key, valueSelector(item));
            }
            return ret;
        }

        [DebuggerStepThrough]
        public static IDictionary<TKey, TValue> Copy<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict == null ? null : dict.ToDictionary(k => k.Key, k => k.Value);
        }

        /// <summary>
        /// Gets the value for the given key, or a default value if the key is not found.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of return value</typeparam>
        /// <param name="dictionary">The dictionary contains the data</param>
        /// <param name="key">The key of value to be retrieved</param>
        /// <returns>The value identified by the key in the collection. Or null if collection is null or no such key exists</returns>
        [DebuggerStepThrough]
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetOrDefault(dictionary, key, default(TValue));
        }


        [DebuggerStepThrough]
        public static TValue FallbackGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params TKey[] keys)
        {
            if (!dictionary.IsNullOrEmpty())
            {
                foreach (var key in keys)
                {
                    if (dictionary.ContainsKey(key))
                    {
                        return dictionary[key];
                    }
                }
            }
            return default(TValue);
        }

        /// <summary>
        /// Convert a collection to dictionary with the safe option skipDupe
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="TKey">key type of the dict</typeparam>
        /// <param name="collection">the collection</param>
        /// <param name="keySelector">function to get key from item</param>
        /// <param name="skipDupe">set to true if want to safely skip duplications</param>
        /// <returns>dictionary</returns>
        [DebuggerStepThrough]
        public static IDictionary<TKey, T> ToDictionary<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector, bool skipDupe)
        {
            return ToDictionary(collection, keySelector, item => item, skipDupe);
        }

        [DebuggerStepThrough]
        public static IDictionary<TKey, T> EmptyIfNull<TKey, T>(this IDictionary<TKey, T> dict)
        {
            return dict ?? new Dictionary<TKey, T>();
        }

        [DebuggerStepThrough]
        public static TValue SafeGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.IsNullOrEmpty())
            {
                return defaultValue;
            }
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// ad dictionary with a new one, with skipping the existing items.s
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IDictionary<TKey, T> Merge<TKey, T>(this IDictionary<TKey, T> dict, IDictionary<TKey, T> other)
        {
            if (other == null)
            {
                return dict;
            }
            if (dict == null)
            {
                dict = new Dictionary<TKey, T>();
            }
            foreach (var item in other)
            {
                dict.AddOrSkip(item.Key, item.Value);
            }
            return dict;
        }
    }
}
