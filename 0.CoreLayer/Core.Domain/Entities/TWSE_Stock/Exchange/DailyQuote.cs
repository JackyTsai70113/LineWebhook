using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Domain.Entities.TWSE_Stock.Exchange {

    public class DailyQuote {

        public DailyQuote() {
            CreateDateTime = DateTime.Now;
        }

        public DateTime CreateDateTime { get; set; }

        [Key, Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [MaxLength(8)]
        public string StockCode { get; set; }

        #region 元/股

        /// <summary>
        /// 成交股數
        /// </summary>
        [Display(Name = "成交股數")]
        public int TradeVolume { get; set; }

        /// <summary>
        /// 成交筆數
        /// </summary>
        [Display(Name = "成交筆數")]
        public int Transaction { get; set; }

        /// <summary>
        /// 成交金額
        /// </summary>
        [Display(Name = "成交金額")]
        public int TradeValue { get; set; }

        /// <summary>
        /// 開盤價
        /// </summary>
        [Display(Name = "開盤價")]
        public float OpeningPrice { get; set; }

        /// <summary>
        /// 最高價
        /// </summary>
        [Display(Name = "最高價")]
        public float HighestPrice { get; set; }

        /// <summary>
        /// 最低價
        /// </summary>
        [Display(Name = "最低價")]
        public float LowestPrice { get; set; }

        /// <summary>
        /// 收盤價
        /// </summary>
        [Display(Name = "收盤價")]
        public float ClosingPrice { get; set; }

        /// <summary>
        /// 漲跌(+/-)
        /// </summary>
        [Display(Name = "漲跌(+/-)")]
        public StockDirectionEnum Direction { get; set; }

        /// <summary>
        /// 漲跌價差
        /// </summary>
        [Display(Name = "漲跌價差")]
        public float Change { get; set; }

        #endregion 元/股

        #region 交易單位

        /// <summary>
        /// 最後揭示買價
        /// </summary>
        [Display(Name = "最後揭示買價")]
        public float LastBestBidPrice { get; set; }

        /// <summary>
        /// 最後揭示買量
        /// </summary>
        [Display(Name = "最後揭示買量")]
        public int LastBestBidVolume { get; set; }

        /// <summary>
        /// 最後揭示賣價
        /// </summary>
        [Display(Name = "最後揭示賣價")]
        public float LastBestAskPrice { get; set; }

        /// <summary>
        /// 最後揭示賣量
        /// </summary>
        [Display(Name = "最後揭示賣量")]
        public int LastBestAskVolume { get; set; }

        /// <summary>
        /// 本益比
        /// </summary>
        [Display(Name = "本益比")]
        public float PriceEarningRatio { get; set; }

        #endregion 交易單位
    }
}