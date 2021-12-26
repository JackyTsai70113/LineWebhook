namespace Core.Domain.DTO.TWSE {
    /// <summary>
    /// Twse Open Api 的回傳Model
    /// </summary>
    /// <remarks>
    /// https://openapi.twse.com.tw/#/%E8%AD%89%E5%88%B8%E4%BA%A4%E6%98%93/get_holidaySchedule_holidaySchedule
    /// </remarks>
    public class HolidaySchedule {
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        public string Day { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string Description { get; set; }
    }
}