using Core.Domain.Interafaces.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Utilities {

    public static class RangeUtility {
    }

    /// <summary>
    /// 時間區間
    /// </summary>
    public class DateTimeRange : IRange<DateTime> {

        public DateTimeRange(DateTime start, DateTime end) {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        /// <summary>
        /// 是否包含指定值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否包含</returns>
        public bool Includes(DateTime value) {
            return (Start <= value) && (value <= End);
        }

        /// <summary>
        /// 是否包含區間
        /// </summary>
        /// <param name="range">區間</param>
        /// <returns>是否包含</returns>
        public bool Includes(IRange<DateTime> range) {
            return (Start <= range.Start) && (range.End <= End);
        }

        /// <summary>
        /// 回傳此時間區間內的每一天
        /// </summary>
        /// <returns>DateTime的IEnumerable</returns>
        /// <remarks>可再改到IRange裡面</remarks>
        public IEnumerable<DateTime> EachDay() {
            for (DateTime day = Start; day.Date <= End; day = day.AddDays(1)) {
                yield return day;
            }
        }

        /// <summary>
        /// 回傳此時間區間內的每一工作天
        /// </summary>
        /// <returns>DateTime的IEnumerable</returns>
        /// <remarks>可再改到IRange裡面</remarks>
        public IEnumerable<DateTime> EachWorkDay() {
            for (DateTime day = Start; day.Date <= End; day = day.AddDays(1)) {
                if (day.IsWeekend()) {
                    continue;
                }
                yield return day;
            }
        }
    }
}