using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class ReflectionExtensions
    {
        public static Stream GetResourceStreamFromExecutingAssembly(this string name)
        {
            //the calling assembly is actually the one which wants the stream
            return Assembly.GetCallingAssembly().GetManifestResourceStream(name);
        }

        public static string GetResourceTextFromExecutingAssembly(this string name, bool throwIfNotFound = true)
        {
            var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(name);
            if (stream == null)
            {
                if (throwIfNotFound)
                {
                    throw new ArgumentException(name);
                }
                return null;
            }
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
