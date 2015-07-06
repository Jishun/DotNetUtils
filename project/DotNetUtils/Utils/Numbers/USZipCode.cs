using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public class UsZipCode
    {
        private UsZipCode() { }

        public static UsZipCode Empty
        {
            get
            {
                return new UsZipCode { ZipCode = "     ", ZipCodeExtension = "    " };
            }
        }

        public static UsZipCode FromString(string raw)
        {
            if (raw.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("raw");
            }
            var pureNumberZip = raw.ExtractPureNumber();
            if (pureNumberZip.Length != 5 && pureNumberZip.Length != 9)
            {
                throw new ArgumentException("Invalid zip code");
            }

            var zip = new UsZipCode();
            if (pureNumberZip.Length == 5)
            {
                zip.ZipCode = pureNumberZip;
                zip.ZipCodeExtension = string.Empty;
            }
            else
            {
                zip.ZipCode = pureNumberZip.Substring(0, 5);
                zip.ZipCodeExtension = pureNumberZip.Substring(5);
            }
            return zip;
        }

        public string ZipCode { get; private set; }

        public string ZipCodeExtension { get; private set; }
    }
}
