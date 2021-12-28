using Core.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Domain.Entities.TWSE_Stock.Exchange {

    /// <summary>
    /// 年度交易資訊
    /// </summary>
    public class YearlyTradingInformation : EntityBase {

        public YearlyTradingInformation() : base() {
        }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        [Column(TypeName = "smallint")]
        [Display(Name = "年份")]
        public short Year { get; set; }

        /// <summary>
        /// 股票代號
        /// </summary>
        [Required]
        [MaxLength(8)]
        [Display(Name = "股票代號")]
        public string StockCode { get; set; }

        /// <summary>
        /// 成交股數
        /// </summary>
        [Display(Name = "成交股數")]
        public long TradeVolume { get; set; }

        /// <summary>
        /// 成交金額
        /// </summary>
        [Display(Name = "成交金額")]
        public long TradeValue { get; set; }

        /// <summary>
        /// 成交筆數
        /// </summary>
        [Display(Name = "成交筆數")]
        public int Transaction { get; set; }

        /// <summary>
        /// 最高價
        /// </summary>
        [Display(Name = "最高價")]
        public float HighestPrice { get; set; }

        /// <summary>
        /// 最高價日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "最高價日期")]
        public DateTime HighestPriceDate { get; set; }

        /// <summary>
        /// 最低價
        /// </summary>
        [Display(Name = "最低價")]
        public float LowestPrice { get; set; }

        /// <summary>
        /// 最低價日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "最低價日期")]
        public DateTime LowestPriceDate { get; set; }

        /// <summary>
        /// 收盤平均價
        /// </summary>
        [Display(Name = "收盤平均價")]
        public float AverageClosingPrice { get; set; }
    }
}