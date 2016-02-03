using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Cache of EST Time zone
        /// </summary>
        private static TimeZoneInfo easternTimeZoneInfo;

        /// <summary>
        /// Gets US Eastern Standard Time Zone
        /// </summary>
        public static TimeZoneInfo USEasternTimeZoneInfo
        {
            get
            {
                if (easternTimeZoneInfo == null)
                {
                    easternTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                }
                return easternTimeZoneInfo;
            }
        }

        /// <summary>
        /// Get the closet date for target DayOfWeek
        /// </summary>
        /// <param name="date">The original date</param>
        /// <param name="target">The target of DayOfWeek</param>
        /// <returns>The closet date which is target day of week</returns>
        public static DateTime GetClosetDate(this DateTime date, DayOfWeek target)
        {
            if (date.DayOfWeek == target)
            {
                return date;
            }
            if (date.DayOfWeek > target)
            {
                return date.AddDays(target - date.DayOfWeek).Date;
            }
            else
            {
                return date.AddDays(target - date.DayOfWeek - 7).Date;
            }
        }

        /// <summary>
        /// Get the current month start date
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current month start date</returns>
        public static DateTime GetMonthStartDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Get the current month end date
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current month start date</returns>
        public static DateTime GetMonthEndDate(this DateTime date)
        {
            var monthStateDate = date.GetMonthStartDate();
            return monthStateDate.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Get the current quarter start date
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current quarter start date</returns>
        public static DateTime GetQuarterStartDate(this DateTime date)
        {
            var year = date.Year;
            switch (date.Month)
            {
                case 1:
                case 2:
                case 3:
                    return new DateTime(year, 1, 1);
                case 4:
                case 5:
                case 6:
                    return new DateTime(year, 4, 1);
                case 7:
                case 8:
                case 9:
                    return new DateTime(year, 7, 1);
                case 10:
                case 11:
                case 12:
                    return new DateTime(year, 10, 1);
                default:
                    throw new InvalidOperationException("date's month out of range");
            }
        }

        /// <summary>
        /// Get the current quarter end date
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current quarter end date</returns>
        public static DateTime GetQuarterEndDate(this DateTime date)
        {
            var year = date.Year;
            switch (date.Month)
            {
                case 1:
                case 2:
                case 3:
                    return new DateTime(year, 3, 31);
                case 4:
                case 5:
                case 6:
                    return new DateTime(year, 6, 30);
                case 7:
                case 8:
                case 9:
                    return new DateTime(year, 9, 30);
                case 10:
                case 11:
                case 12:
                    return new DateTime(year, 12, 31);
                default:
                    throw new InvalidOperationException("date's month out of range");
            }
        }

        /// <summary>
        /// Get the current quarter end month
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current quarter end date</returns>
        public static int GetQuarterEndMonth(this DateTime date)
        {
            return date.GetQuarterEndDate().Month;
        }

        /// <summary>
        /// Get the current quarter
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current quarter</returns>
        public static int GetQuarter(this DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        /// <summary>
        /// Get the current year start date
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>The current quarter start date</returns>
        public static DateTime GetYearStartDate(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime GetYearEndDate(this DateTime date)
        {
            return new DateTime(date.Year, 12, 31);
        }

        /// <summary>
        /// Get the short form name of the specified month
        /// </summary>
        private static readonly string[] MonthNameSort = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public static string GetMonthNameShort(this DateTime date)
        {
            return MonthNameSort[date.Month - 1];
        }

        /// <summary>
        /// To string which contains only month and day
        /// </summary>
        public static string ToShortMonthDate(this DateTime date)
        {
            return "{0}/{1}".FormatInvariantCulture(date.Month, date.Day);
        }

        /// <summary>
        /// Get the next week day
        /// </summary>
        /// <param name="date">The original date</param>
        /// <returns>A date which is the next week day</returns>
        public static DateTime GetNextWeekDay(this DateTime date)
        {
            var nextDate = date.AddDays(1);
            while (nextDate.DayOfWeek == DayOfWeek.Saturday || nextDate.DayOfWeek == DayOfWeek.Sunday)
            {
                nextDate = nextDate.AddDays(1);
            }
            return nextDate;
        }

        /// <summary>
        /// Convert the datetime to EST timezone
        /// </summary>
        /// <param name="dateTime">The date time to be converted</param>
        /// <returns>
        ///     A DateTime instance stands for EST
        /// </returns>
        public static DateTime UtcTimeToESTTime(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime is not in Utc kind");
            }
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, USEasternTimeZoneInfo);
        }

        /// <summary>
        /// Get the Julian date from date time instance
        /// </summary>
        /// <param name="dateTime">The datetime instance</param>
        /// <returns>The Julian date</returns>
        public static int ToJulianDate(this DateTime dateTime)
        {
            return dateTime.Year * 1000 + dateTime.DayOfYear;
        }

        /// <summary>
        /// Get last month string representation of specific quarter
        /// </summary>
        /// <param name="ancharDate">The date as anchor</param>
        /// <returns>The string representation of last month</returns>
        public static string GetQuarterLastMonth(this DateTime ancharDate)
        {
            switch (ancharDate.GetQuarter())
            {
                case 1:
                    return "03";
                case 2:
                    return "06";
                case 3:
                    return "09";
                case 4:
                    return "12";
                default:
                    throw new ArgumentOutOfRangeException("quarter");
            }
        }

        public static DateTime ParseDateTime(this object src, string format = null)
        {
            var date = src.ParseDateTimeNullable(format);
            if (date.HasValue)
            {
                return date.Value;
            }
            throw new InvalidDataException("Unable to parse date from {0} with format {1}".FormatInvariantCulture(src, format));
        }

        public static DateTime? ParseDateTimeNullable(this object src, string format = null)
        {
            if (src == null)
            {
                return null;
            }
            if (src is DateTime)
            {
                return (DateTime)src;
            }
            DateTime date;
            if (format == null)
            {
                if (DateTime.TryParse(src.ToString(), out date))
                {
                    return date;
                }
            }
            else
            {
                if (DateTime.TryParseExact(src.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out date))
                {
                    return date;
                }
            }
            return null;
        }

    }
}
