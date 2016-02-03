using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class NumberToEnglishText
    {
        private static readonly string[] Ones =
        {
            "Zero",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine"
        };

        private static readonly string[] Teens =
        {
            "Ten",
            "Eleven",
            "Twelve",
            "Thirteen",
            "Fourteen",
            "Fifteen",
            "Sixteen",
            "Seventeen",
            "Eighteen",
            "Nineteen"
        };

        private static readonly string[] Tens =
        {
            String.Empty,
            "Ten",
            "Twenty",
            "Thirty",
            "Forty",
            "Fifty",
            "Sixty",
            "Seventy",
            "Eighty",
            "Ninety"
        };

        // US:
        private static readonly string[] Thousands =
        {
            String.Empty,
            "Thousand",
            "Million",
            "Billion",
            "Trillion",
            "Quadrillion"
        };

        /// <summary>
        /// Converts a numeric value to words suitable for the portion of
        /// a check that writes out the amount.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <returns>A string converted from a decimal value</returns>
        public static string Convert(decimal value)
        {
            var allZeros = true;

            // Use StringBuilder to build result
            var builder = new StringBuilder();
            // Convert integer portion of value to string
            var digits = ((long)value).ToString();
            // Traverse characters in reverse order
            for (var i = digits.Length - 1; i >= 0; i--)
            {
                var ndigit = (int)(digits[i] - '0');
                var column = (digits.Length - (i + 1));

                // Determine if ones, tens, or hundreds column
                string temp;
                switch (column % 3)
                {
                    case 0:        // Ones position
                        var showThousands = true;
                        if (i == 0)
                        {
                            // First digit in number (last in loop)
                            temp = "{0} ".FormatInvariantCulture(Ones[ndigit]);
                        }
                        else if (digits[i - 1] == '1')
                        {
                            // This digit is part of "teen" value
                            temp = "{0} ".FormatInvariantCulture(Teens[ndigit]);
                            // Skip tens position
                            i--;
                        }
                        else if (ndigit != 0)
                        {
                            // Any non-zero digit
                            temp = "{0} ".FormatInvariantCulture(Ones[ndigit]);
                        }
                        else
                        {
                            // This digit is zero. If digit in tens and hundreds
                            // column are also zero, don't show "thousands"
                            temp = String.Empty;
                            // Test for non-zero digit in this grouping
                            showThousands = digits[i - 1] != '0' || (i > 1 && digits[i - 2] != '0');
                        }

                        // Show "thousands" if non-zero in grouping
                        if (showThousands)
                        {
                            if (column > 0)
                            {
                                temp = "{0}{1}{2}".FormatInvariantCulture(temp, Thousands[column / 3], allZeros ? " " : ", ");
                            }
                            // Indicate non-zero digit encountered
                            allZeros = false;
                        }
                        builder.Insert(0, temp);
                        break;

                    case 1:        // Tens column
                        if (ndigit > 0)
                        {
                            temp = "{0}{1}".FormatInvariantCulture(Tens[ndigit], (digits[i + 1] != '0') ? "-" : " ");
                            builder.Insert(0, temp);
                        }
                        break;

                    case 2:        // Hundreds column
                        if (ndigit > 0)
                        {
                            temp = "{0} Hundred ".FormatInvariantCulture(Ones[ndigit]);
                            builder.Insert(0, temp);
                        }
                        break;
                }
            }

            // Append fractional portion/cents
            var cent = (int) ((value - (long) value)*100);
            if (cent > 0)
            {
                builder.AppendFormat("and {0:00} Cents ", cent);
            }
            builder.AppendFormat("Only");
            // Capitalize first letter
            return "{0}{1}".FormatInvariantCulture(Char.ToUpper(builder[0]), builder.ToString(1, builder.Length - 1));
        }
    }
}
