using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// Translate a list of DayOfWeek value to a short integer value
        /// </summary>
        /// <param name="values">The list of DayOfWeek values</param>
        /// <returns>A short integer value represents the list</returns>
        public static short GenerateDaysOfWeekValue(IEnumerable<DayOfWeek> values)
        {
            short retFlag = 0x0000; //0x0000
            foreach (var value in values)
            {
                switch (value)
                {
                    case DayOfWeek.Monday:
                        retFlag |= 0x0001; //0000000000000001
                        break;
                    case DayOfWeek.Tuesday:
                        retFlag |= 0x0002; //0000000000000010
                        break;
                    case DayOfWeek.Wednesday:
                        retFlag |= 0x0004; //0000000000000100
                        break;
                    case DayOfWeek.Thursday:
                        retFlag |= 0x0008; //0000000000001000
                        break;
                    case DayOfWeek.Friday:
                        retFlag |= 0x0010; //0000000000010000
                        break;
                    case DayOfWeek.Saturday:
                        retFlag |= 0x0020; //0000000000100000
                        break;
                    case DayOfWeek.Sunday:
                        retFlag |= 0x0040; //0000000001000000
                        break;
                }
            }
            return retFlag;
        }

        /// <summary>
        /// Parse a short integer value to a list of DayOfWeek value
        /// </summary>
        /// <param name="value">The value to be parsed</param>
        /// <returns>A list of DayOfWeek value</returns>
        public static IList<DayOfWeek> ParseDaysOfWeekValue(short value)
        {
            IList<DayOfWeek> retList = new List<DayOfWeek>(7);
            if ((value & 0x0001) == 0x0001) //0000000000000001
            {
                retList.Add(DayOfWeek.Monday);
            }
            if ((value & 0x0002) == 0x0002)//0000000000000010
            {
                retList.Add(DayOfWeek.Tuesday);
            }
            if ((value & 0x0004) == 0x0004)//0000000000000100
            {
                retList.Add(DayOfWeek.Wednesday);
            }
            if ((value & 0x0008) == 0x0008)//0000000000001000
            {
                retList.Add(DayOfWeek.Thursday);
            }
            if ((value & 0x0010) == 0x0010)//0000000000010000
            {
                retList.Add(DayOfWeek.Friday);
            }
            if ((value & 0x0020) == 0x0020)//0000000000100000
            {
                retList.Add(DayOfWeek.Saturday);
            }
            if ((value & 0x0040) == 0x0040)//0000000001000000
            {
                retList.Add(DayOfWeek.Sunday);
            }
            return retList;
        }

        /// <summary>
        /// Translate a list of days of Month value to a string
        /// </summary>
        /// <param name="daysOfMonth">The list of values</param>
        /// <returns>A string representation of the values, in value1, value2, value3, ... format</returns>
        public static string GenerateDaysOfMonthString(IEnumerable<ushort> daysOfMonth)
        {
            var collection = new CommaDelimitedStringCollection();
            collection.AddRange(daysOfMonth.Select(day => day.ToString()).ToArray());
            return collection.ToString();
        }

        /// <summary>
        /// Parse a string value in format of value1, value2, value3,... to be a list of short integer represents the days in month
        /// </summary>
        /// <param name="values">The string representation</param>
        /// <returns>A list of short integer represents days in month</returns>
        public static IEnumerable<ushort> ParseDaysOfMonthValue(string values)
        {
            if (values.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("values");
            }
            return values.Split(Constants.CommaDelimCharArray, StringSplitOptions.RemoveEmptyEntries).Select(day => Convert.ToUInt16(day.Trim()));
        }

        /// <summary>
        /// Count the number of weekday during <paramref name="startDate"/> and <paramref name="endDate"/>
        /// </summary>
        /// <param name="startDate">The startDate</param>
        /// <param name="endDate">The endDate</param>
        /// <param name="interval">The week interval</param>
        /// <returns>count</returns>
        public static int GetWeekdayCount(DateTime startDate, DateTime endDate, int interval = 1)
        {
            return GetDaysOfWeekCount(startDate, endDate, interval, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                      DayOfWeek.Thursday, DayOfWeek.Friday);
        }

        /// <summary>
        /// Calculate the count day in the given collection in specified range 
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate"> end date</param>
        /// <param name="interval">The week interval</param>
        /// <param name="daysOfWeek">the day collection</param>
        /// <returns>count</returns>
        public static int GetDaysOfWeekCount(DateTime startDate, DateTime endDate, int interval, params DayOfWeek[] daysOfWeek)
        {
            if (interval < 1)
            {
                throw new ArgumentOutOfRangeException("interval");
            }
            var total = (endDate.Date - startDate.Date).Days + 1;
            if (total <= 0)
            {
                return 0;
            }
            var weeks = total / (7 * interval);
            var remainder = total % (7 * interval);
            var newRemainder = remainder % 7;
            if (newRemainder == 0 && remainder > 0)
            {
                return (weeks + 1) * daysOfWeek.Count();
            }
            var days = 0;
            while (newRemainder > 0)
            {
                days += daysOfWeek.Any(d => d == startDate.DayOfWeek) ? 1 : 0;
                startDate = startDate.AddDays(1);
                newRemainder--;
            }
            return days + weeks * daysOfWeek.Count();
        }

    }
}
