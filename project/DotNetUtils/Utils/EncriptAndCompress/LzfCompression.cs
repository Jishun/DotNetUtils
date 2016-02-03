using System;

namespace DotNetUtils
{
    /// <summary>
    /// Improved C# LZF Compressor, a very small data compression library. The compression algorithm is extremely fast. 
    /// </summary>
    public class LzfCompression
    {
        private readonly long[] _hashTable = new long[Hsize];

        private const uint Hlog = 14;
        private const uint Hsize = (1 << 14);
        private const uint MaxLit = (1 << 5);
        private const uint MaxOff = (1 << 13);
        private const uint MaxRef = ((1 << 8) + (1 << 3));

        /// <summary>
        /// Compresses the data using LibLZF algorithm
        /// </summary>
        /// <param name="input">Reference to the data to compress</param>
        /// <param name="inputLength">Length of the data to compress</param>
        /// <param name="output">Reference to a buffer which will contain the compressed data</param>
        /// <param name="outputLength">Length of the compression buffer (should be bigger than the input buffer)</param>
        /// <returns>The size of the compressed archive in the output buffer</returns>
        public int Compress(byte[] input, int inputLength, byte[] output, int outputLength)
        {
            Array.Clear(_hashTable, 0, (int)Hsize);

            uint iidx = 0;
            uint oidx = 0;

            var hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]); // FRST(in_data, iidx);
            var lit = 0;

            for (; ; )
            {
                if (iidx < inputLength - 2)
                {
                    hval = (hval << 8) | input[iidx + 2];
                    long hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - Hlog)) - hval * 5) & (Hsize - 1));
                    var reference = _hashTable[hslot];
                    _hashTable[hslot] = iidx;


                    long off;
                    if ((off = iidx - reference - 1) < MaxOff
                        && iidx + 4 < inputLength
                        && reference > 0
                        && input[reference + 0] == input[iidx + 0]
                        && input[reference + 1] == input[iidx + 1]
                        && input[reference + 2] == input[iidx + 2]
                        )
                    {
                        /* match found at *reference++ */
                        uint len = 2;
                        var maxlen = (uint)inputLength - iidx - len;
                        maxlen = maxlen > MaxRef ? MaxRef : maxlen;

                        if (oidx + lit + 1 + 3 >= outputLength)
                        {return 0;}

                        do
                        {len++;}
                        while (len < maxlen && input[reference + len] == input[iidx + len]);

                        if (lit != 0)
                        {
                            output[oidx++] = (byte)(lit - 1);
                            lit = -lit;
                            do
                            {output[oidx++] = input[iidx + lit];}
                            while ((++lit) != 0);
                        }

                        len -= 2;
                        iidx++;

                        if (len < 7)
                        {
                            output[oidx++] = (byte)((off >> 8) + (len << 5));
                        }
                        else
                        {
                            output[oidx++] = (byte)((off >> 8) + (7 << 5));
                            output[oidx++] = (byte)(len - 7);
                        }

                        output[oidx++] = (byte)off;

                        iidx += len - 1;
                        hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);

                        hval = (hval << 8) | input[iidx + 2];
                        _hashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - Hlog)) - hval * 5) & (Hsize - 1))] = iidx;
                        iidx++;

                        hval = (hval << 8) | input[iidx + 2];
                        _hashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - Hlog)) - hval * 5) & (Hsize - 1))] = iidx;
                        iidx++;
                        continue;
                    }
                }
                else if (iidx == inputLength)
                {break;}

                /* one more literal byte we must copy */
                lit++;
                iidx++;

                if (lit == MaxLit)
                {
                    if (oidx + 1 + MaxLit >= outputLength)
                    {return 0;}

                    output[oidx++] = (byte)(MaxLit - 1);
                    lit = -lit;
                    do
                    {output[oidx++] = input[iidx + lit];}
                    while ((++lit) != 0);
                }
            }

            if (lit != 0)
            {
                if (oidx + lit + 1 >= outputLength)
                {return 0;}

                output[oidx++] = (byte)(lit - 1);
                lit = -lit;
                do
                {output[oidx++] = input[iidx + lit];}
                while ((++lit) != 0);
            }

            return (int)oidx;
        }


        /// <summary>
        /// Decompresses the data using LibLZF algorithm
        /// </summary>
        /// <param name="input">Reference to the data to decompress</param>
        /// <param name="inputLength">Length of the data to decompress</param>
        /// <param name="output">Reference to a buffer which will contain the decompressed data</param>
        /// <param name="outputLength">The size of the decompressed archive in the output buffer</param>
        /// <returns>Returns decompressed size</returns>
        public int Decompress(byte[] input, int inputLength, byte[] output, int outputLength)
        {
            uint iidx = 0;
            uint oidx = 0;

            do
            {
                uint ctrl = input[iidx++];

                if (ctrl < (1 << 5)) /* literal run */
                {
                    ctrl++;

                    if (oidx + ctrl > outputLength)
                    {
                        //SET_ERRNO (E2BIG);
                        return 0;
                    }

                    do
                    {output[oidx++] = input[iidx++];}
                    while ((--ctrl) != 0);
                }
                else /* back reference */
                {
                    var len = ctrl >> 5;

                    var reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);

                    if (len == 7)
                    {len += input[iidx++];}

                    reference -= input[iidx++];

                    if (oidx + len + 2 > outputLength)
                    {
                        //SET_ERRNO (E2BIG);
                        return 0;
                    }

                    if (reference < 0)
                    {
                        //SET_ERRNO (EINVAL);
                        return 0;
                    }

                    output[oidx++] = output[reference++];
                    output[oidx++] = output[reference++];

                    do
                    {output[oidx++] = output[reference++];}
                    while ((--len) != 0);
                }
            }
            while (iidx < inputLength);

            return (int)oidx;
        }
    }
}
