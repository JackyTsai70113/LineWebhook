using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities.TWSE_Stock {

    /// <summary>
    /// 股利分派情形
    /// </summary>
    public class DividendDistribution {

        /// <summary>
        /// 股票代號
        /// </summary>
        public StockCodeEnum StockCode { get; set; }

        /// <summary>
        /// 當期股利
        /// </summary>
        public float RecentDividend { get; set; }

        /// <summary>
        /// 5年平均股利
        /// </summary>
        public float DividendIn5Years { get; set; }

        /// <summary>
        /// 10年平均股利
        /// </summary>
        public float DividendIn10Years { get; set; }

        /// <summary>
        /// 便宜價(當期)
        /// </summary>
        public float RecentCheapPrice { get; set; }

        /// <summary>
        /// 合理價(當期)
        /// </summary>
        public float RecentReasonablePrice { get; set; }

        /// <summary>
        /// 昂貴價(當期)
        /// </summary>
        public float RecentExpensivePrice { get; set; }

        /// <summary>
        /// 便宜價(十年平均)
        /// </summary>
        public float CheapPriceIn10Years { get; set; }

        /// <summary>
        /// 合理價(十年平均)
        /// </summary>
        public float ReasonablePriceIn10Years { get; set; }

        /// <summary>
        /// 昂貴價(十年平均)
        /// </summary>
        public float ExpensivePriceIn10Years { get; set; }
    }
}