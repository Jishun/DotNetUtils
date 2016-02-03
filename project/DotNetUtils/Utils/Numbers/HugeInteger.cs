using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public sealed class HugeInteger
    {
        private readonly IList<byte> bits;

        public int Length => bits.Count;

        public HugeInteger()
        {
            bits = new List<byte>();
        }

        public byte ForceGet(uint step)
        {
            return step >= Length ? (byte)0 : bits[(int)step];
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int minLength)
        {
            string ret = bits.Aggregate(string.Empty, (agg, bit) => bit.ToString(CultureInfo.InvariantCulture) + agg);
            return ret.Length < minLength ? ret.PadLeft(minLength, '0') : ret;
        }

        public static HugeInteger operator +(HugeInteger a, HugeInteger b)
        {
            int carry = 0;
            HugeInteger ret = new HugeInteger();
            for (uint i = 0; i < a.Length || i < b.Length || carry != 0; i++)
            {
                int sum = a.ForceGet(i) + b.ForceGet(i) + carry;
                ret.bits.Add((byte)(sum % 10));
                carry = sum > 9 ? 1 : 0;
            }
            return ret;
        }

        public static implicit operator HugeInteger(string val)
        {
            return Parse(val);
        }

        public static implicit operator HugeInteger(long val)
        {
            return Parse(val);
        }

        public static HugeInteger Parse(long raw)
        {
            var ret = new HugeInteger();
            while (raw != 0)
            {
                ret.bits.Add((byte)(raw % 10));
                raw /= 10;
            }
            return ret;
        }

        public static HugeInteger Parse(string raw)
        {
            HugeInteger ret = new HugeInteger();
            if (!raw.IsPureNumber())
            {
                throw new ArgumentOutOfRangeException(nameof(raw));
            }
            for (int i = raw.Length - 1; i >= 0; i--)
            {
                byte a = byte.Parse(raw.Substring(i, 1));
                ret.bits.Add(a);
            }
            return ret;
        }
    }
}
