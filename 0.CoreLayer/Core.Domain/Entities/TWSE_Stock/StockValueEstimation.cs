using Core.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Domain.Entities.TWSE_Stock {

    /// <summary>
    /// 股票價值估算
    /// </summary>
    public class StockValueEstimation : EntityBase {

        /// <summary>
        /// 股票代號
        /// </summary>
        [Key]
        [Required]
        [MaxLength(8)]
        public string StockCode { get; set; }

        #region 股利

        /// <summary>
        /// 當期股利
        /// </summary>
        [Display(Name = "當期股利")]
        public float RecentDividends { get; set; }

        /// <summary>
        /// 5年平均股利
        /// </summary>
        [Display(Name = "5年平均股利")]
        public float DividendsIn5Years { get; set; }

        /// <summary>
        /// 10年平均股利
        /// </summary>
        [Display(Name = "10年平均股利")]
        public float DividendsIn10Years { get; set; }

        /// <summary>
        /// 便宜價(當期股利)
        /// </summary>
        [Display(Name = "便宜價(當期股利)")]
        public float CheapPriceByRecentDividends {
            get {
                return RecentDividends * 15;
            }
        }

        /// <summary>
        /// 合理價(當期股利)
        /// </summary>
        [Display(Name = "合理價(當期股利)")]
        public float ReasonablePriceByRecentDividends {
            get {
                return RecentDividends * 20;
            }
        }

        /// <summary>
        /// 昂貴價(當期股利)
        /// </summary>
        [Display(Name = "昂貴價(當期股利)")]
        public float ExpensivePriceByRecentDividends {
            get {
                return RecentDividends * 30;
            }
        }

        /// <summary>
        /// 便宜價(歷史5年股利法)
        /// </summary>
        [Display(Name = "便宜價(歷史5年股利法)")]
        public float CheapPriceByDividendsIn5Years {
            get {
                return DividendsIn5Years * 15;
            }
        }

        /// <summary>
        /// 合理價(歷史5年股利法)
        /// </summary>
        [Display(Name = "合理價(歷史5年股利法)")]
        public float ReasonablePriceByDividendsIn5Years {
            get {
                return DividendsIn5Years * 20;
            }
        }

        /// <summary>
        /// 昂貴價(歷史5年股利法)
        /// </summary>
        [Display(Name = "昂貴價(歷史5年股利法)")]
        public float ExpensivePriceByDividendsIn5Years {
            get {
                return DividendsIn5Years * 30;
            }
        }

        /// <summary>
        /// 便宜價(歷史10年股利法)
        /// </summary>
        [Display(Name = "便宜價(歷史10年股利法)")]
        public float CheapPriceByDividendsIn10Years {
            get {
                return DividendsIn10Years * 15;
            }
        }

        /// <summary>
        /// 合理價(歷史10年股利法)
        /// </summary>
        [Display(Name = "合理價(歷史10年股利法)")]
        public float ReasonablePriceByDividendsIn10Years {
            get {
                return DividendsIn10Years * 20;
            }
        }

        /// <summary>
        /// 昂貴價(歷史10年股利法)
        /// </summary>
        [Display(Name = "昂貴價(歷史10年股利法)")]
        public float ExpensivePriceByDividendsIn10Years {
            get {
                return DividendsIn10Years * 30;
            }
        }

        #endregion 股利

        #region 股價

        /// <summary>
        /// 便宜價(歷史10年股價法)
        /// </summary>
        [Display(Name = "便宜價(歷史10年股價法)")]
        public float CheapPriceByStockPriceOver10Years { get; set; }

        /// <summary>
        /// 合理價(歷史10年股價法)
        /// </summary>
        [Display(Name = "合理價(歷史10年股價法)")]
        public float ReasonablePriceByStockPriceOver10Years { get; set; }

        /// <summary>
        /// 昂貴價(歷史10年股價法)
        /// </summary>
        [Display(Name = "昂貴價(歷史10年股價法)")]
        public float ExpensivePriceByStockPriceOver10Years { get; set; }

        #endregion 股價
    }
}