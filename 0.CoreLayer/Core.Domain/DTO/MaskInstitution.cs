using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO {

    public class MaskInstitution {

        /// <summary>
        /// 醫事機構代碼
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 醫事機構名稱
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 醫事機構地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 醫事機構電話
        /// </summary>
        public string PhoneNumber { set; get; }

        /// <summary>
        /// 成人口罩剩餘數
        /// </summary>
        public int numberOfAdultMasks { get; set; }

        /// <summary>
        /// 兒童口罩剩餘數
        /// </summary>
        public int numberOfChildMasks { set; get; }

        /// <summary>
        /// 來源資料時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}