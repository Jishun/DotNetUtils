using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class LzfHelper
    {
        public static byte[] Compress(string src)
        {
            if (src.IsNullOrWhiteSpace())
            {
                return null;
            }
            var lzf = new LzfCompression();
            var srcData = Encoding.UTF8.GetBytes(src);
            var buff = new byte[srcData.Length];
            var compressedLength = lzf.Compress(srcData, srcData.Length, buff, buff.Length);
            var output = new byte[compressedLength + 4];
            Array.Copy(BitConverter.GetBytes(srcData.Length), 0, output, 0, 4);
            Array.Copy(buff, 0, output, 4, compressedLength);
            return output;
        }

        public static string DecompressAsString(byte[] data)
        {
            if (data.IsNullOrEmpty())
            {
                return null;
            }
            var srcLength = BitConverter.ToInt32(data, 0);
            var buff = new byte[srcLength];
            var lzf = new LzfCompression();
            lzf.Decompress(data.Skip(4).ToArray(), data.Length - 4, buff, srcLength);
            var str = Encoding.UTF8.GetString(buff);
            return str;
        }
    }
}
