using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class NumberUtils
    {

        /// <summary>
        /// Get magnitude of a decimal value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The magnitude of the value</returns>
        public static decimal GetMagnitude(decimal value)
        {
            var intValue = (int)Math.Abs(value);
            int count = 0;
            while (intValue / 10 != 0)
            {
                count++;
                intValue = intValue / 10;
            }
            decimal ret = (intValue + 1);
            while (count-- > 0)
            {
                ret = ret * 10;
            }
            return value > 0 ? ret : -ret;
        }

        /// <summary>
        /// Calculate the least common multiplier
        /// </summary>
        /// <param name="a">number 1</param>
        /// <param name="b">number 2</param>
        /// <returns>least common multiplier</returns>
        public static uint LCM(uint a, uint b)
        {
            return a * b / GCD(a, b);
        }

        /// <summary>
        /// calculate the Great common divisor  
        /// </summary>
        /// <param name="a">number 1</param>
        /// <param name="b">number 2</param>
        /// <returns>Great common divisor</returns>
        public static uint GCD(uint a, uint b)
        {
            uint r;
            if (a > b)
            {
                b = b ^ a;
                a = b ^ a;
                b = b ^ a;
            }
            while ((r = b % a) != 0)
            {
                b = a;
                a = r;
            }
            return r;
        }

    }
}
