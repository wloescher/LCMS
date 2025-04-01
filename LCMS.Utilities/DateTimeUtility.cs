namespace LCMS.Utilities
{
    public static class DateTimeUtility
    {
        /// <summary>
        /// Convert Unix time to DateTime.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns>DateTime.</returns>
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Convert DateTime to Unix time.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>UNIX time as Int64.</returns>
        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        /// <summary>
        /// Get elapsed time.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>Elapsed time as string.</returns>
        public static string GetElapsedTime(TimeSpan timeSpan)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timeSpan.Hours,
                timeSpan.Minutes,
                timeSpan.Seconds,
                timeSpan.Milliseconds / 10);
        }
    }
}
