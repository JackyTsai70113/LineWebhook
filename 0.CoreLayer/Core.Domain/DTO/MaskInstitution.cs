using System;
using System.ComponentModel;

namespace Core.Domain.DTO {

    public class MaskInstitution {

        /// <summary>
        /// 醫事機構代碼
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 醫事機構名稱
        /// </summary>
        [DisplayName("機構名")]
        public string Name { set; get; }

        /// <summary>
        /// 醫事機構地址
        /// </summary>
        [DisplayName("地址")]
        public string Address { get; set; }

        /// <summary>
        /// 醫事機構電話
        /// </summary>
        [DisplayName("電話")]
        public string PhoneNumber { set; get; }

        /// <summary>
        /// 成人口罩剩餘數
        /// </summary>
        [DisplayName("成人口罩數")]
        public int NumberOfAdultMasks { get; set; }

        /// <summary>
        /// 兒童口罩剩餘數
        /// </summary>
        [DisplayName("兒童口罩數")]
        public int NumberOfChildMasks { set; get; }

        /// <summary>
        /// 來源資料時間
        /// </summary>
        [DisplayName("更新時間")]
        public DateTime UpdateTime { get; set; }
    }
}