using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetUtils
{

    public static class StreamExtensions
    {
        public static string ReadLine(this Stream inputStream)
        {
            var data = String.Empty;
            while (true)
            {
                var nextChar = inputStream.ReadByte();
                if (nextChar == '\n') { break; }
                if (nextChar == '\r') { continue; }
                if (nextChar == -1) { continue; };
                data += Convert.ToChar(nextChar);
            }
            return data;
        }

        public static IDictionary<string, string> ReadHttpHeaders(Stream inputStream)
        {
            var ret = new Dictionary<string, string>();
            Console.WriteLine("readHeaders()");
            string line;
            while ((line = inputStream.ReadLine()) != null)
            {
                if (line.Equals(""))
                {
                    break;
                }
                var index = line.IndexOf(':');
                if (index == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                ret[line.Substring(0, index).Trim()] = line.Substring(index + 1).Trim();
            }
            return ret;
        }
    }
}
