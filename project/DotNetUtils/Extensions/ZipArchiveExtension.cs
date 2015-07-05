using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class ZipArchiveExtension
    {
        public static byte[] SaveZipFile(this IDictionary<string, byte[]> files)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var en = zip.CreateEntry(file.Key);
                        using (var stream = en.Open())
                        {
                            stream.Write(file.Value, 0, file.Value.Length);
                        }
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
