using System;
using System.Diagnostics;

namespace MultiServerLibrary.Extension
{
    public static class DateTimeUtils
    {
        private static readonly Stopwatch _swTicker = Stopwatch.StartNew();
        private static readonly long _swTickerInitialTicks = DateTime.UtcNow.Ticks;

        #region Time

        public static DateTime GetHighPrecisionUtcTime()
        {
            return new DateTime(_swTicker.Elapsed.Ticks + _swTickerInitialTicks, DateTimeKind.Utc);
        }

        public static long GetMillisecondsSinceStartup()
        {
            return _swTicker.ElapsedMilliseconds;
        }

        public static uint GetUnixTimeU32()
        {
            return GetHighPrecisionUtcTime().ToUnixTimeU32();
        }

        public static long GetUnixTime()
        {
            return GetHighPrecisionUtcTime().ToUnixTime();
        }

        // Prone to the year 2106 problem: https://en.wikipedia.org/wiki/Year_2038_problem#Year_2106_problem
        public static uint ToUnixTimeU32(this DateTime time)
        {
            return (uint)time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long ToUnixTime(this DateTime time)
        {
            return (time.Kind == DateTimeKind.Utc
                 ? new DateTimeOffset(time)
                 : new DateTimeOffset(time.ToUniversalTime())).ToUnixTimeSeconds();
        }

        public static DateTime ToUtcDateTime(this uint unixTime)
        {
            return new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(unixTime);
        }

        public static string GetCurrentUnixTimestampAsString()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public static bool IsAprilFoolsDay()
        {
            const byte april = 4;
            var today = DateTime.Now;
            return today.Month == april && today.Day == april;
        }

        #endregion
    }
}
