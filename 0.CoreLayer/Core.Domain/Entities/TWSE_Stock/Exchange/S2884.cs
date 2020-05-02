using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities.TWSE_Stock.Exchange {

    /// <summary>
    /// 玉山金 2884
    /// </summary>
    public class S2884 {

        #region 基本資料

        /// <summary>
        /// 證券代號
        /// </summary>
        public string StockCode { get; } = "2884";

        /// <summary>
        /// 證券名稱
        /// </summary>
        public string StockName { get; } = "玉山金";

        #endregion 基本資料

        #region 股

        /// <summary>
        /// 成交股數
        /// </summary>
        public int TradeVolume { get; set; }

        /// <summary>
        /// 成交筆數
        /// </summary>
        public int Transaction { get; set; }

        /// <summary>
        /// 成交金額
        /// </summary>
        public int TradeValue { get; set; }

        /// <summary>
        /// 開盤價
        /// </summary>
        public float OpeningPrice { get; set; }

        /// <summary>
        /// 最高價
        /// </summary>
        public float HighestPrice { get; set; }

        /// <summary>
        /// 最低價
        /// </summary>
        public float LowestPrice { get; set; }

        /// <summary>
        /// 收盤價
        /// </summary>
        public float ClosingPrice { get; set; }

        /// <summary>
        /// 漲跌(+/-)
        /// </summary>
        public StockDirectionEnum Direction { get; set; }

        /// <summary>
        /// 漲跌價差
        /// </summary>
        public float Change { get; set; }

        #endregion 股

        #region 交易單位

        /// <summary>
        /// 最後揭示買價
        /// </summary>
        public float LastBestBidPrice { get; set; }

        /// <summary>
        /// 最後揭示買量
        /// </summary>
        public int LastBestBidVolume { get; set; }

        /// <summary>
        /// 最後揭示賣價
        /// </summary>
        public float LastBestAskPrice { get; set; }

        /// <summary>
        /// 最後揭示賣量
        /// </summary>
        public int LastBestAskVolume { get; set; }

        /// <summary>
        /// 本益比
        /// </summary>
        public float PriceEarningRatio { get; set; }

        #endregion 交易單位
    }
}