using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BL.Interfaces.TWSE_Stock;
using BL.Services;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {

    public class StockController : Controller {

        public StockController() {
            DailyQuoteService = new DailyQuoteService();
            DividendDistributionService = new DividendDistributionService();
            StockValueEstimationService = new StockValueEstimationService();
        }

        public IDailyQuoteService DailyQuoteService { get; set; }
        public IDividendDistributionService DividendDistributionService { get; set; }
        public IStockValueEstimationService StockValueEstimationService { get; set; }

        public IActionResult Index() {
            IActionResult actionResult = CrawlForStockValueEstimationIn10YearsAndSave();
            return actionResult;
        }

        #region 每日報表 DailyQuote

        /// <summary>
        /// 資料有誤時可指定這隻api進行資料的更新
        /// </summary>
        /// <returns></returns>
        public IActionResult CrawlDailyQuoteListAndSaveByDate() {
            List<DateTime> dateTimes = new List<DateTime>();
            int successDailyQuoteNumber = 0;
            dateTimes.Add(new DateTime(2020, 08, 26));
            foreach (var date in dateTimes) {
                successDailyQuoteNumber += DailyQuoteService.CrawlDailyQuoteListAndUpdateByDate(
                    date, StockCategoryEnum.FinancialAndInsurance);
            }
            return Ok(successDailyQuoteNumber);
        }

        public IActionResult CrawlDailyQuoteListAndSaveByMonth(string dateStr = null) {
            DateTime date = DateTime.Now;
            if (dateStr != null) {
                if (!dateStr.TryParse(out date)) {
                    throw new Exception("字串格式錯誤，無法轉型為日期.");
                }
            }

            int successDailyQuoteNumber = DailyQuoteService.CrawlDailyQuoteListAndInsertByMonth(
                date, StockCategoryEnum.FinancialAndInsurance);
            return Ok(successDailyQuoteNumber);
        }

        public IActionResult CrawlDailyQuoteListAndInsertByYear() {
            int successDailyQuoteNumber2010 = DailyQuoteService.CrawlDailyQuoteListAndInsertByYear(
                2010, StockCategoryEnum.FinancialAndInsurance);
            return Ok(successDailyQuoteNumber2010);
        }

        public IActionResult GetDailyQuoteByDate() {
            List<DailyQuote> dailyQuoteList = DailyQuoteService.GetDailyQuoteByDate(new DateTime(2010, 1, 6));
            return Ok(dailyQuoteList);
        }

        public IActionResult GetDailyQuoteByDateAndStockCode() {
            DailyQuote dailyQuote = DailyQuoteService.GetFirstDailyQuoteByDateAndStockCode(new DateTime(2020, 5, 8), "2884");
            return Ok(dailyQuote);
        }

        #endregion 每日報表 DailyQuote

        #region 股利分派 DividendDistribution

        public IActionResult GetDividendDistributionIn10Years() {
            List<DividendDistribution> dividendDistributionList = DividendDistributionService.CrawlDividendDistributionListByStockCodeEnumArray();
            return Ok(dividendDistributionList);
        }

        #endregion 股利分派 DividendDistribution

        #region 股票價值估算 StockValueEstimation

        public IActionResult CrawlForStockValueEstimationIn10YearsAndSave() {
            List<StockCodeEnum> stockCodeEnums = Enum.GetValues(typeof(StockCodeEnum)).Cast<StockCodeEnum>().ToList();
            var result = StockValueEstimationService.CrawlForStockValueEstimationIn10YearsAndSave(
                stockCodeEnums.GetRange(0, 6));
            return Ok(result);
        }

        #endregion 股票價值估算 StockValueEstimation
    }
}