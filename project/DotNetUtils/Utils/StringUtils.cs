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
