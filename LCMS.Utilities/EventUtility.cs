namespace LCMS.Utilities
{
    public static class EventUtility
    {
        #region Public Methods

        /// <summary>
        /// Get hourly series or target dates (e.g. every hour between 8am and 6pm).
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 1 month worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate equals DateTime.MinValue.</remarks>
        public static List<DateTime> GetHourlySeriesTargetDates(int frequency, DateTime startTime, DateTime endTime, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = new DateTime(seriesStartDate.Year, seriesStartDate.Month, seriesStartDate.Day, startTime.Hour, startTime.Minute, startTime.Second);
            AddHourlySeriesTargetDate(0, startTime, endTime, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddHourlySeriesTargetDate(frequency, startTime, endTime, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddHourlySeriesTargetDate(frequency, startTime, endTime, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 1 month worth of dates
                seriesEndDate = seriesStartDate.AddMonths(1);
                while (targetDate < seriesEndDate)
                {
                    AddHourlySeriesTargetDate(frequency, startTime, endTime, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get daily series of target dates by interval (e.g. every day).
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 6 months worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetDailySeriesTargetDatesByInterval(int frequency, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            targetDates.Add(targetDate);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddDailySeriesTargetDateByInterval(frequency, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddDailySeriesTargetDateByInterval(frequency, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 6 month worth of dates
                seriesEndDate = seriesStartDate.AddMonths(6);
                while (targetDate < seriesEndDate)
                {
                    AddDailySeriesTargetDateByInterval(frequency, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get daily series of target dates by weekday.
        /// </summary>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 6 months worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetDailySeriesTargetDatesEveryWeekday(DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Get initial target date
            DateTime targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddDailySeriesTargetDateEveryWeekday(seriesEndDate, ref targetDate, ref targetDates, false);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddDailySeriesTargetDateEveryWeekday(DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddDailySeriesTargetDateEveryWeekday(seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 6 month worth of dates
                seriesEndDate = seriesStartDate.AddMonths(6);
                while (targetDate < seriesEndDate)
                {
                    AddDailySeriesTargetDateEveryWeekday(seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get weekly series of target dates (e.g. every week on Tuesdays and Thursdays).
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="targetDays"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 1 year worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetWeeklySeriesTargetDates(int frequency, List<DayOfWeek> targetDays, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            if (targetDays == null) { throw new ArgumentNullException(nameof(targetDays)); }

            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddWeeklySeriesTargetDate(0, targetDays, seriesMaxItems, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddWeeklySeriesTargetDate(frequency, targetDays, seriesMaxItems, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddWeeklySeriesTargetDate(frequency, targetDays, seriesMaxItems, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 1 year worth of dates
                seriesEndDate = seriesStartDate.AddYears(1);
                while (targetDate < seriesEndDate)
                {
                    AddWeeklySeriesTargetDate(frequency, targetDays, seriesMaxItems, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get monthly series of target dates by day number (e.g. 1st day of every month).
        /// </summary>
        /// <param name="dayNumber"></param>
        /// <param name="frequency"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 3 years worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetMonthlySeriesTargetDatesByDayNumber(int dayNumber, int frequency, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddMonthlySeriesTargetDateByDayNumber(dayNumber, 0, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddMonthlySeriesTargetDateByDayNumber(dayNumber, frequency, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddMonthlySeriesTargetDateByDayNumber(dayNumber, frequency, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 3 year worth of dates
                seriesEndDate = seriesStartDate.AddYears(3);
                while (targetDate < seriesEndDate)
                {
                    AddMonthlySeriesTargetDateByDayNumber(dayNumber, frequency, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get monthly series of target dates by day name (e.g. 1st Monday of every month).
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="frequency"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 3 years worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetMonthlySeriesTargetDatesByDayName(int instance, DayOfWeek dayOfWeek, int frequency, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddMonthlySeriesTargetDateByDayName(instance, dayOfWeek, 0, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddMonthlySeriesTargetDateByDayName(instance, dayOfWeek, frequency, seriesStartDate, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddMonthlySeriesTargetDateByDayName(instance, dayOfWeek, frequency, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 3 years worth of dates
                seriesEndDate = seriesStartDate.AddYears(3);
                while (targetDate < seriesEndDate)
                {
                    AddMonthlySeriesTargetDateByDayName(instance, dayOfWeek, frequency, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get quarterly series of target dates by day number (e.g. 1st day of every quarter).
        /// </summary>
        /// <param name="dayNumber"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 3 years worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetQuarterlySeriesTargetDatesByDayNumber(int dayNumber, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialQuarterlyTargetDateForQuarter(seriesStartDate, seriesTargetTime, dayNumber);
            targetDates.Add(targetDate);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddQuarterlySeriesTargetDateByDayNumber(dayNumber, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddQuarterlySeriesTargetDateByDayNumber(dayNumber, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 3 year worth of dates
                seriesEndDate = seriesStartDate.AddYears(3);
                while (targetDate < seriesEndDate)
                {
                    AddQuarterlySeriesTargetDateByDayNumber(dayNumber, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get quarterly series of target dates by day name (e.g. 1st Monday of every quarter).
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 3 years worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetQuarterlySeriesTargetDatesByDayName(int instance, DayOfWeek dayOfWeek, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddQuarterlySeriesTargetDateByDayName(instance, dayOfWeek, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems)
                {
                    AddQuarterlySeriesTargetDateByDayName(instance, dayOfWeek, seriesStartDate, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddQuarterlySeriesTargetDateByDayName(instance, dayOfWeek, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 3 years worth of dates
                seriesEndDate = seriesStartDate.AddYears(3);
                while (targetDate < seriesEndDate)
                {
                    AddQuarterlySeriesTargetDateByDayName(instance, dayOfWeek, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get yearly series of target dates by date (e.g. Recur every year on January 1st).
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 10 years worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetYearlySeriesTargetDatesByDate(int frequency, int month, int day, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Check for date outside of monthly max
            if ((month == 2 && day > 29) // Feb
                || ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30) // Apr, Jun, Sep, Nov
                || day > 31) // Jan, Mar, May, Jul, Aug, Oct, Dec
            {
                return targetDates;
            }

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddYearlySeriesTargetDateByDate(0, month, day, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems && targetDate.Year + frequency < DateTime.MaxValue.Year)
                {
                    AddYearlySeriesTargetDateByDate(frequency, month, day, seriesStartDate, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue && targetDate.Year + frequency < DateTime.MaxValue.Year)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddYearlySeriesTargetDateByDate(frequency, month, day, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 10 years worth of dates
                seriesEndDate = seriesStartDate.AddYears(10);
                while (targetDate < seriesEndDate && targetDate.Year + frequency < DateTime.MaxValue.Year)
                {
                    AddYearlySeriesTargetDateByDate(frequency, month, day, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get yearly series of target dates by day name (e.g. recur every year on the 1st Monday of January).
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="instance"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="month"></param>
        /// <param name="seriesStartDate"></param>
        /// <param name="seriesMaxItems"></param>
        /// <param name="seriesEndDate"></param>
        /// <param name="seriesTargetTime"></param>
        /// <returns></returns>
        /// <remarks>No limit will return 10 years worth of dates.</remarks>
        /// <remarks>No limit occurrs when seriesMaxItems is less than or equal to 0 and seriesEndDate eauals DateTime.MinValue.</remarks>
        public static List<DateTime> GetYearlySeriesTargetDatesByDayName(int frequency, int instance, DayOfWeek dayOfWeek, int month, DateTime seriesStartDate, int seriesMaxItems, DateTime seriesEndDate, DateTime seriesTargetTime)
        {
            var targetDates = new List<DateTime>();

            // Add initial target date
            var targetDate = GetInitialTargetDate(seriesStartDate, seriesTargetTime);
            AddYearlySeriesTargetDateByDayName(0, instance, dayOfWeek, month, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);

            // Determine series pattern
            if (seriesMaxItems > 0)
            {
                // Limit by number of items
                while (targetDates.Count < seriesMaxItems && targetDate.Year + frequency < DateTime.MaxValue.Year)
                {
                    AddYearlySeriesTargetDateByDayName(frequency, instance, dayOfWeek, month, seriesStartDate, DateTime.MinValue, ref targetDate, ref targetDates);
                }
            }
            else if (seriesEndDate > DateTime.MinValue && targetDate.Year + frequency < DateTime.MaxValue.Year)
            {
                // Limit by end date
                while (targetDate <= seriesEndDate)
                {
                    AddYearlySeriesTargetDateByDayName(frequency, instance, dayOfWeek, month, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }
            else
            {
                // No limit specified - create 10 years worth of dates
                seriesEndDate = seriesStartDate.AddYears(10);
                while (targetDate < seriesEndDate && targetDate.Year + frequency < DateTime.MaxValue.Year)
                {
                    AddYearlySeriesTargetDateByDayName(frequency, instance, dayOfWeek, month, seriesStartDate, seriesEndDate, ref targetDate, ref targetDates);
                }
            }

            return targetDates;
        }

        /// <summary>
        /// Get Nth weekday of month after start date.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="nthWeek"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static DateTime GetNthWeekdayOfMonth(DateTime startDate, int nthWeek, DayOfWeek dayOfWeek)
        {
            var nthDate = new DateTime(startDate.Year, startDate.Month, 1, startDate.Hour, startDate.Minute, startDate.Second);
            nthDate = nthDate.Next(dayOfWeek);
            nthDate = nthDate.AddDays((nthWeek - 1) * 7);

            // Check for Nth date before requested date
            if (nthDate < startDate)
            {
                // Move to next month
                nthDate = startDate.AddMonths(1);
                nthDate = new DateTime(nthDate.Year, nthDate.Month, 1, nthDate.Hour, nthDate.Minute, nthDate.Second);
                nthDate = nthDate.Next(dayOfWeek);
                nthDate = nthDate.AddDays((nthWeek - 1) * 7);
            }

            // Check for last day of week in month
            if (nthWeek == 5 && nthDate.Month != startDate.Month)
            {
                nthDate = new DateTime(startDate.Year, startDate.Month, 1, startDate.Hour, startDate.Minute, startDate.Second);
                nthDate = nthDate.Next(dayOfWeek);
                nthDate = nthDate.AddDays((nthWeek - 2) * 7);
            }

            return nthDate;
        }

        /// <summary>
        /// Get Nth weekday of quarter after start date.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="nthWeek"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static DateTime GetNthWeekdayOfQuarter(DateTime startDate, int nthWeek, DayOfWeek dayOfWeek)
        {
            //// Check for start date that is the correct weekday
            //if (startDate.DayOfWeek != dayOfWeek || (startDate.Month != 1 && startDate.Month != 4 && startDate.Month != 7 && startDate.Month != 10))
            //{
            //    // Get target date for NEXT quarter
            //    var year = startDateQuarter == 4 ? startDate.Year + 1 : startDate.Year;
            //    var month = startDateQuarter == 4 ? 1 : startDateQuarter + 1;
            //    startDateForQuarter = new DateTime(year, month, startDate.Day, startDate.Hour, startDate.Minute, startDate.Second);
            //}

            var quarter = (startDate.Month + 2) / 3;
            var firstDayOfQuarter = new DateTime(startDate.Year, (quarter - 1) * 3 + 1, 1);
            var nthDate = new DateTime(firstDayOfQuarter.Year, firstDayOfQuarter.Month, 1, firstDayOfQuarter.Hour, firstDayOfQuarter.Minute, firstDayOfQuarter.Second);
            nthDate = nthDate.Next(dayOfWeek);
            nthDate = nthDate.AddDays((nthWeek - 1) * 7);

            // Check for Nth date before requested date
            if (nthDate <= startDate)
            {
                // Move to next quarter
                nthDate = firstDayOfQuarter.AddMonths(3);
                nthDate = new DateTime(nthDate.Year, nthDate.Month, 1, startDate.Hour, startDate.Minute, startDate.Second);
                nthDate = nthDate.Next(dayOfWeek);
                nthDate = nthDate.AddDays((nthWeek - 1) * 7);
            }

            // Check for last day of week in month
            if (nthWeek == 5 && nthDate.Month != startDate.Month)
            {
                // Get specified day from the 5th week
                nthDate = new DateTime(startDate.Year, startDate.Month, 1, startDate.Hour, startDate.Minute, startDate.Second);
                nthDate = nthDate.Next(dayOfWeek);
                nthDate = nthDate.AddDays(4 * 7);

                // Check for quarter ending month
                if (nthDate.Month % 3 != 0)
                {
                    // Get specified day from the 4th week
                    nthDate = new DateTime(startDate.Year, startDate.Month, 1, startDate.Hour, startDate.Minute, startDate.Second);
                    nthDate = nthDate.Next(dayOfWeek);
                    nthDate = nthDate.AddDays(3 * 7);
                }
            }

            return nthDate;
        }

        #endregion

        #region Private Methods

        private static void AddHourlySeriesTargetDate(int frequency, DateTime startTime, DateTime endTime, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            targetDate = targetDate.AddHours(frequency);

            // Check for a Start Time later then End Time
            if (startTime > endTime)
            {
                // Swap start and end time
                var newStartTime = endTime;
                var newEndTime = startTime;
                startTime = newStartTime;
                endTime = newEndTime;
            }

            // Check to see if target date falls between target times and is no later than series end date
            if ((targetDate.TimeOfDay >= startTime.TimeOfDay && (endTime == DateTime.MinValue || targetDate.TimeOfDay <= endTime.TimeOfDay))
                && (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate))
            {
                targetDates.Add(targetDate);
            }
            else
            {
                // Check to see if day has already been added to target dates
                if (targetDate.DayOfYear == targetDates.Last().DayOfYear && endTime != DateTime.MinValue)
                {
                    targetDate = targetDate.AddDays(1);
                }

                // Set target date start time
                targetDate = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, startTime.Hour, startTime.Minute, startTime.Second);

                // Check to see if target date falls between target times and is no later than series end date
                if ((targetDate.TimeOfDay >= startTime.TimeOfDay && targetDate.TimeOfDay <= endTime.TimeOfDay)
                    && (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate))
                {
                    // Check to make sure target date hasn't already been added to collection
                    if (!targetDates.Contains(targetDate))
                    {
                        targetDates.Add(targetDate);
                    }
                }
                else
                {
                    // Increment target date to prevent infinite loop
                    targetDate = targetDate.AddSeconds(1);
                }
            }
        }

        private static void AddDailySeriesTargetDateByInterval(int frequency, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            targetDate = targetDate.AddDays(frequency);

            // Check to make sure target date is no later than series end date
            if (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate)
            {
                targetDates.Add(targetDate);
            }
        }

        private static void AddDailySeriesTargetDateEveryWeekday(DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates, bool incrementTargetDay = true)
        {
            // Increment target date
            if (incrementTargetDay)
            {
                targetDate = targetDate.AddDays(1);
            }

            // Verify that target date is a weekday
            switch (targetDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    break;
                default:
                    // Check to make sure target date is no later than series end date
                    if (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate)
                    {
                        targetDates.Add(targetDate);
                    }
                    break;
            }
        }

        private static void AddWeeklySeriesTargetDate(int frequency, List<DayOfWeek> targetDays, int seriesMaxItems, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            var weekIncrement = frequency <= 1 ? 0 : frequency - 1;
            targetDate = targetDate.AddDays(weekIncrement * 7);

            // Set target date week end date
            var targetDateWeekEndingDate = targetDate.AddDays(7);
            targetDateWeekEndingDate = new DateTime(targetDateWeekEndingDate.Year, targetDateWeekEndingDate.Month, targetDateWeekEndingDate.Day, 0, 0, 0);
            while (targetDate <= targetDateWeekEndingDate)
            {
                // Check to see if the DAY of target date is one of the specified target days and is no later than series end date
                if (targetDays.Contains(targetDate.DayOfWeek))
                {
                    if (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate)
                    {
                        targetDates.Add(targetDate);

                        // Check to see if we've reache the max number of items
                        if (seriesMaxItems > 0 && targetDates.Count == seriesMaxItems)
                        {
                            targetDate = new DateTime(targetDateWeekEndingDate.Year, targetDateWeekEndingDate.Month, targetDateWeekEndingDate.Day, targetDate.Hour, targetDate.Minute, targetDate.Second);
                            break;
                        }
                    }
                }
                targetDate = targetDate.AddDays(1);
            }
        }

        private static void AddMonthlySeriesTargetDateByDayNumber(int dayNumber, int frequency, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            targetDate = targetDate.AddMonths(frequency);

            // Check target date day number 
            if (targetDate.Day > dayNumber)
            {
                targetDate = targetDate.AddMonths(1);
            }

            // Check for valid day number for given month
            if (DateTime.DaysInMonth(targetDate.Year, targetDate.Month) >= dayNumber)
            {
                targetDate = new DateTime(targetDate.Year, targetDate.Month, dayNumber, targetDate.Hour, targetDate.Minute, targetDate.Second);

                // Check to make sure target date is no later than series end date
                if (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate)
                {
                    targetDates.Add(targetDate);
                }
            }
        }

        private static void AddMonthlySeriesTargetDateByDayName(int instance, DayOfWeek dayOfWeek, int frequency, DateTime seriesStartDate, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Check to see if we need to increment target date
            if (frequency == 0)
            {
                targetDate = GetNthWeekdayOfMonth(targetDate, instance, dayOfWeek);
            }
            else
            {
                var incrementedTargtDate = targetDate.AddMonths(frequency);
                var firstDayOfIncrementedTargetDate = new DateTime(incrementedTargtDate.Year, incrementedTargtDate.Month, 1, incrementedTargtDate.Hour, incrementedTargtDate.Minute, incrementedTargtDate.Second);
                targetDate = GetNthWeekdayOfMonth(firstDayOfIncrementedTargetDate, instance, dayOfWeek);
            }

            // Check to make sure target date is no later than series end date
            if (targetDate >= seriesStartDate && (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate))
            {
                targetDates.Add(targetDate);
            }
        }

        private static void AddQuarterlySeriesTargetDateByDayNumber(int dayNumber, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            targetDate = targetDate.AddMonths(3);

            // Check for valid day number for given month
            if (DateTime.DaysInMonth(targetDate.Year, targetDate.Month) >= dayNumber)
            {
                targetDate = new DateTime(targetDate.Year, targetDate.Month, dayNumber, targetDate.Hour, targetDate.Minute, targetDate.Second);

                // Check to make sure target date is no later than series end date
                if (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate)
                {
                    targetDates.Add(targetDate);
                }
            }
        }

        private static void AddQuarterlySeriesTargetDateByDayName(int instance, DayOfWeek dayOfWeek, DateTime seriesStartDate, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            targetDate = GetNthWeekdayOfQuarter(targetDate, instance, dayOfWeek);

            // Check to see if we need to increment target date
            if (targetDates.Contains(targetDate))
            {
                targetDate = GetNthWeekdayOfQuarter(targetDate.AddMonths(3), instance, dayOfWeek);
            }

            // Check to make sure target date is no later than series end date
            if (targetDate >= seriesStartDate && (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate))
            {
                targetDates.Add(targetDate);
            }
        }

        private static void AddYearlySeriesTargetDateByDate(int frequency, int month, int day, DateTime seriesStartDate, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            if (DateTime.DaysInMonth(targetDate.Year + frequency, month) >= day)
            {
                // Increment target date
                targetDate = new DateTime(targetDate.Year + frequency, month, day, targetDate.Hour, targetDate.Minute, targetDate.Second);

                // Check to make sure target date is no later than series end date
                if (targetDate >= seriesStartDate && (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate))
                {
                    targetDates.Add(targetDate);
                }
            }
            else
            {
                targetDate = targetDate.AddYears(frequency);
            }
        }

        private static void AddYearlySeriesTargetDateByDayName(int frequency, int instance, DayOfWeek dayOfWeek, int month, DateTime seriesStartDate, DateTime seriesEndDate, ref DateTime targetDate, ref List<DateTime> targetDates)
        {
            // Increment target date
            targetDate = targetDate.AddYears(frequency);

            // Get specified month
            targetDate = new DateTime(targetDate.Year, month, 1, targetDate.Hour, targetDate.Minute, targetDate.Second);

            // Get specified weekday instance
            targetDate = GetNthWeekdayOfMonth(targetDate, instance, dayOfWeek);

            // Check to make sure target date is no later than series end date
            if (targetDate >= seriesStartDate && (seriesEndDate == DateTime.MinValue || targetDate < seriesEndDate))
            {
                targetDates.Add(targetDate);
            }
        }

        private static DateTime GetInitialTargetDate(DateTime seriesStartDate, DateTime seriesTargetTime)
        {
            return new DateTime(seriesStartDate.Year, seriesStartDate.Month, seriesStartDate.Day, seriesTargetTime.Hour, seriesTargetTime.Minute, seriesTargetTime.Second);
        }

        private static DateTime GetInitialQuarterlyTargetDateForQuarter(DateTime seriesStartDate, DateTime seriesTargetTime, int dayNumber)
        {
            var targetDate = new DateTime(seriesStartDate.Year, seriesStartDate.Month, seriesStartDate.Day, seriesTargetTime.Hour, seriesTargetTime.Minute, seriesTargetTime.Second);
            var targetDateQuarter = (targetDate.Month + 2) / 3;
            var firstDayOfTargetDateQuarter = new DateTime(targetDate.Year, (targetDateQuarter - 1) * 3 + 1, 1);

            // Check for target date that is the first day of a quarter
            if (targetDate != firstDayOfTargetDateQuarter)
            {
                // Get target date for NEXT quarter
                var year = targetDateQuarter == 4 ? seriesStartDate.Year + 1 : seriesStartDate.Year;
                var month = targetDateQuarter == 4 ? 1 : targetDateQuarter + 1;
                targetDate = new DateTime(year, month, dayNumber, seriesTargetTime.Hour, seriesTargetTime.Minute, seriesTargetTime.Second);
            }

            return targetDate;
        }

        ///<summary>Gets the first week day following a date.</summary>
        ///<param name="date">The date.</param>
        ///<param name="dayOfWeek">The day of week to return.</param>
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
        private static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }

        #endregion
    }
}
