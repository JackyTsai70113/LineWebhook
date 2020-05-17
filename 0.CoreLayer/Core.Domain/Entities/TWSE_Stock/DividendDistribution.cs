using Core.Domain.Entities.Base;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core.Domain.Entities.TWSE_Stock {

    /// <summary>
    /// 股利分派
    /// </summary>
    public class DividendDistribution : EntityBase {

        public DividendDistribution() : base() {
        }

        /// <summary>
        /// 股票代號
        /// </summary>
        [Required]
        [MaxLength(8)]
        public string StockCode { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        /// <remarks>範圍: -2^15 (-32,768) 到 2^15-1 (32,767)</remarks>
        [Required]
        [Column(TypeName = "smallint")]
        public short Year { get; set; }

        /// <summary>
        /// 盈餘分配之現金股利(元/股)
        /// </summary>
        public float CashDividendsToBeDistributedFromRetainedEarnings { get; set; }

        /// <summary>
        /// 法定盈餘公積、資本公積之現金股利(元/股)
        /// </summary>
        public float CashDividendsFromLegalReserveAndCapitalSurplus { get; set; }

        /// <summary>
        /// 盈餘轉增資配股(元/股)
        /// </summary>
        public float SharesDistributedFromEarnings { get; set; }

        /// <summary>
        /// 法定盈餘公積、資本公積轉增資配股(元/股)
        /// </summary>
        public float SharesDistributedFromLegalReserveAndCapitalSurplus { get; set; }

        /// <summary>
        /// 合計股利
        /// </summary>
        [NotMapped]
        public float Dividends {
            get {
                return GetZeroIfNotValid(CashDividendsToBeDistributedFromRetainedEarnings)
                    + GetZeroIfNotValid(CashDividendsFromLegalReserveAndCapitalSurplus)
                    + GetZeroIfNotValid(SharesDistributedFromEarnings)
                    + GetZeroIfNotValid(SharesDistributedFromLegalReserveAndCapitalSurplus);
            }
        }

        private static float GetZeroIfNotValid(float floatNumber) {
            if (floatNumber == -1f) {
                return 0;
            }
            return floatNumber;
        }
    }
}