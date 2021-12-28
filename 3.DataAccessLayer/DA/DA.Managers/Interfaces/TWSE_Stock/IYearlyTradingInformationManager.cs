using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Managers.Interfaces.TWSE_Stock {

    public interface IYearlyTradingInformationManager {

        /// <summary>
        /// 根據 股票代號 抓取年度交易資訊列表
        /// </summary>
        /// <param name="stockCodeEnum">股票代號</param>
        /// <returns>年度交易資訊列表</returns>
        List<YearlyTradingInformation> CrawlYearlyTradingInformation(StockCodeEnum stockCodeEnum);
    }
}