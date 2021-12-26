﻿using System;
using System.Collections.Generic;
using BL.Services.Providers;

namespace BL.Services.Holiday {
    /// <summary>
    /// 營業日相關Helper
    /// </summary>

    public static class HolidayHelper {
        /// <summary>
        /// 取得最近的n個營業日
        /// </summary>
        /// <param name="count">數量</param>
        /// <param name="startDateTime">開始時間</param>
        /// <returns>營業日列表</returns>
        public static List<DateTime> GetTheMostRecentBusinessDay(
            int count, DateTime startDateTime = new DateTime())
        {
            List<DateTime> theMostRecentBusinessDays = new List<DateTime>();
            DateTime currentDateTime = new DateTime();
            if(startDateTime == new DateTime()){
                startDateTime = DateTime.UtcNow.AddHours(8);
            }
            List<DateTime> holidays = TwseProvider.GetHolidaySchedule();
            while(theMostRecentBusinessDays.Count < count){
                if(IsBusinessDay(currentDateTime, holidays)) {
                    theMostRecentBusinessDays.Add(currentDateTime);
                }
                currentDateTime = currentDateTime.AddDays(-1);
            }
            return theMostRecentBusinessDays;
        }

        /// <summary>
        /// 此時間是否是營業日
        /// </summary>
        /// <param name="dateTime">時間</param>
        /// <param name="holidays">休市時間</param>
        /// <returns>是否是營業日</returns>
        private static bool IsBusinessDay(DateTime dateTime, List<DateTime> holidays){
            bool isSaturday = dateTime.DayOfWeek.ToString() != "Saturday";
            bool isSunday = dateTime.DayOfWeek.ToString() != "Sunday";
            bool isHoliday = holidays.Contains(dateTime.Date);
            return !isSaturday && !isSunday && !isHoliday;
        }
    }
}