using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public class Base32
    {
        // the valid chars for the encoding
        private const string ValidChars = "QAZ2WSX3EDC4RFV5TGB6YHN7UJM8K9LP";

        /// <summary>
        /// Converts an array of bytes to a Base32-k string.
        /// </summary>
        public static string ToBase32String(byte[] bytes)
        {
            var sb = new StringBuilder();         // holds the base32 chars
            var hi = 5;
            var currentByte = 0;

            while (currentByte < bytes.Length)
            {
                // do we need to use the next byte?
                byte index;
                if (hi > 8)
                {
                    // get the last piece from the current byte, shift it to the right
                    // and increment the byte counter
                    index = (byte)(bytes[currentByte++] >> (hi - 5));
                    if (currentByte != bytes.Length)
                    {
                        // if we are not at the end, get the first piece from
                        // the next byte, clear it and shift it to the left
                        index = (byte)(((byte)(bytes[currentByte] << (16 - hi)) >> 3) | index);
                    }

                    hi -= 3;
                }
                else if (hi == 8)
                {
                    index = (byte)(bytes[currentByte++] >> 3);
                    hi -= 3;
                }
                else
                {
                    // simply get the stuff from the current byte
                    index = (byte)((byte)(bytes[currentByte] << (8 - hi)) >> 3);
                    hi += 5;
                }

                sb.Append(ValidChars[index]);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Converts a Base32-k string into an array of bytes.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Input string <paramref name="s">s</paramref> contains invalid Base32-k characters.
        /// </exception>
        public static byte[] FromBase32String(string str)
        {
            var numBytes = str.Length * 5 / 8;
            var bytes = new byte[numBytes];

            // all UPPERCASE chars
            str = str.ToUpper();

            if (str.Length < 3)
            {
                bytes[0] = (byte)(ValidChars.IndexOf(str[0]) | ValidChars.IndexOf(str[1]) << 5);
                return bytes;
            }

            var bitBuffer = (ValidChars.IndexOf(str[0]) | ValidChars.IndexOf(str[1]) << 5);
            var bitsInBuffer = 10;
            var currentCharIndex = 2;
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)bitBuffer;
                bitBuffer >>= 8;
                bitsInBuffer -= 8;
                while (bitsInBuffer < 8 && currentCharIndex < str.Length)
                {
                    bitBuffer |= ValidChars.IndexOf(str[currentCharIndex++]) << bitsInBuffer;
                    bitsInBuffer += 5;
                }
            }

            return bytes;
        }
    }
}
