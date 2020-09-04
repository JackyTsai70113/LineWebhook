using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Enums {

    /// <summary>
    /// 三大法人Enum
    /// </summary>
    public enum ForeignAndOtherInvestorEnum {

        /// <summary>
        /// 自營商
        /// </summary>
        Dealers = 0,

        /// <summary>
        /// 投信
        /// </summary>
        SecuritiesInvestmentTrustCompanies = 1,

        /// <summary>
        /// 外資及陸資
        /// </summary>
        ForeignInvestors = 2
    }
}