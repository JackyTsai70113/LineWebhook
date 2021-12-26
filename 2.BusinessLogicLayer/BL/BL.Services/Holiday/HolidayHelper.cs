using System;
using System.Collections.Generic;
using BL.Services.Providers;

namespace BL.Services.Holiday {

    public static class HolidayHelper {

        public static List<DateTime> GetTheMostRecentBusinessDay(
            int count, DateTime startDateTime = new DateTime())
        {
            List<DateTime> theMostRecentBusinessDays = new List<DateTime>();
            DateTime currentDateTime = new DateTime();
            if(startDateTime == new DateTime()){
                currentDateTime = DateTime.UtcNow.AddHours(8);
            }
            List<DateTime> holidays = TwseProvider.GetHolidaySchedule();
            while(theMostRecentBusinessDays.Count < count){
                if(holidays.Contains(currentDateTime.Date)
                    || currentDateTime.DayOfWeek.ToString() == "Saturday"
                    || currentDateTime.DayOfWeek.ToString() == "Sunday") {
                    currentDateTime = currentDateTime.AddDays(-1);
                } else {
                    theMostRecentBusinessDays.Add(currentDateTime);
                }
            }
            return theMostRecentBusinessDays;
        }
    }

    public class Holiday {
        public string 西元日期 { get; set; }
        public string 星期 { get; set; }
        public string 是否放假 { get; set; }
        public string 備註 { get; set; }
    }
}