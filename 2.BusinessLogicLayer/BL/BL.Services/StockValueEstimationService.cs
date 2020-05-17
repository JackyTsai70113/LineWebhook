using BL.Interfaces.TWSE_Stock;
using BL.Services.Base;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using DA.Managers.Interfaces.TWSE_Stock;
using DA.Managers.TWSE_Stock;
using DA.Repositories.Interfaces.TWSE_Stock;
using DA.Repositories.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace BL.Services {

    public class StockValueEstimationService : BaseService, IStockValueEstimationService {

        #region IOC

        public StockValueEstimationService() {
            // 抓取股利分派
            DividendDistributionManager = new DividendDistributionManager();
            // 抓取年度交易資訊
            YearlyTradingInformationManager = new YearlyTradingInformationManager();
            // 儲存或更新股利
            DividendDistributionRepository = new DividendDistributionRepository();
            // 儲存或更新股價估計表
            StockValueEstimationRepository = new StockValueEstimationRepository();
            // 儲存或更新年度交易資訊
            YearlyTradingInformationRepository = new YearlyTradingInformationRepository();
        }

        public IDividendDistributionManager DividendDistributionManager { get; set; }

        public IYearlyTradingInformationManager YearlyTradingInformationManager { get; set; }

        public IDividendDistributionRepository DividendDistributionRepository { get; set; }

        public IStockValueEstimationRepository StockValueEstimationRepository { get; set; }
        public IYearlyTradingInformationRepository YearlyTradingInformationRepository { get; set; }

        #endregion IOC

        /// <summary>
        /// 根據 股票代號 抓取股利分派後計算，並分別更新DB: DividendDistribution, YearlyTradingInformation, StockValueEstimation。
        /// </summary>
        /// <param name="stockCodeEnum">股票代號</param>
        /// <returns>儲存數量</returns>
        public int CrawlForStockValueEstimationIn10YearsAndSave(StockCodeEnum stockCodeEnum) {
            int successNumber = 0;

            //根據 股票代號 抓取股利分派列表
            List<DividendDistribution> dividendDistributionList = DividendDistributionManager.CrawlDividendDistribution(stockCodeEnum);
            //儲存 股利分派資料
            DividendDistributionRepository.SetSqlConnection(LineWebhookContextConnectionString);
            successNumber += DividendDistributionRepository.SaveDividendDistributionList(dividendDistributionList);

            //根據 年度交易資訊 抓取股利分派列表
            List<YearlyTradingInformation> yearlyTradingInformationList = YearlyTradingInformationManager.CrawlYearlyTradingInformation(stockCodeEnum);
            //儲存 年度交易資訊列表
            YearlyTradingInformationRepository.SetSqlConnection(LineWebhookContextConnectionString);
            successNumber += YearlyTradingInformationRepository.SaveYearlyTradingInformationList(yearlyTradingInformationList);

            StockValueEstimation stockValueEstimation = GetStockValueEstimationList(dividendDistributionList, yearlyTradingInformationList);
            StockValueEstimationRepository.SetSqlConnection(LineWebhookContextConnectionString);
            successNumber += StockValueEstimationRepository.SaveStockValueEstimation(stockValueEstimation);

            return successNumber;
        }

        /// <summary>
        /// 根據 股票代號列表 抓取股利分派後計算，並分別更新DB: DividendDistribution, YearlyTradingInformation, StockValueEstimation。
        /// </summary>
        /// <param name="stockCodeEnums">股票代號列表</param>
        /// <returns>儲存數量</returns>
        public int CrawlForStockValueEstimationIn10YearsAndSave(IEnumerable<StockCodeEnum> stockCodeEnums) {
            int successNumber = 0;
            foreach (StockCodeEnum stockCodeEnum in stockCodeEnums) {
                successNumber += CrawlForStockValueEstimationIn10YearsAndSave(stockCodeEnum);
            }

            return successNumber;
        }

        protected StockValueEstimation GetStockValueEstimationList(List<DividendDistribution> dividendDistributionList,
                                                                       List<YearlyTradingInformation> yearlyTradingInformationList) {
            StockValueEstimation stockValueEstimation = new StockValueEstimation();

            //先排序
            IOrderedEnumerable<DividendDistribution> DividendDistributionEnumerable = dividendDistributionList.OrderByDescending(x => x.Year);
            IOrderedEnumerable<YearlyTradingInformation> YearlyTradingInformationEnumerable =
                yearlyTradingInformationList.OrderByDescending(x => x.Year);

            stockValueEstimation.StockCode = dividendDistributionList.First().StockCode;

            #region 股利法

            stockValueEstimation.RecentDividends = dividendDistributionList.First().Dividends;
            stockValueEstimation.DividendsIn5Years = dividendDistributionList.Take(5).Average(x => x.Dividends);
            stockValueEstimation.DividendsIn10Years = dividendDistributionList.Take(10).Average(x => x.Dividends);

            #endregion 股利法

            #region 股價法

            stockValueEstimation.CheapPriceByStockPriceOver10Years = yearlyTradingInformationList.Average(x => x.LowestPrice);
            stockValueEstimation.ReasonablePriceByStockPriceOver10Years = yearlyTradingInformationList.Average(x => x.AverageClosingPrice);
            stockValueEstimation.ExpensivePriceByStockPriceOver10Years = yearlyTradingInformationList.Average(x => x.HighestPrice);

            #endregion 股價法

            return stockValueEstimation;
        }
    }
}