using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BL.Services;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Interafaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {

    public class StockController : Controller {

        public StockController() {
            DailyQuoteService = new DailyQuoteService();
        }

        //public DailyQuoteManager MyProperty { get; set; }
        public IDailyQuoteService DailyQuoteService { get; set; }

        public IActionResult Index() {
            IActionResult test = GetDailyQuoteByDate();
            return test;
        }

        public IActionResult CreateDailyQuote() {
            return Ok("");
        }

        public IActionResult GetDailyQuoteList() {
            return Ok("");
        }

        public IActionResult GetDailyQuoteListByMonth() {
            int successDailyQuoteNumber = DailyQuoteService.GetDailyQuoteListByMonthAndSave(
                new DateTime(2012, 5, 10), StockCategoryEnum.FinancialAndInsurance);
            return Ok(successDailyQuoteNumber);
        }

        public IActionResult GetDailyQuoteListByYear() {
            int successDailyQuoteNumber2010 = DailyQuoteService.GetDailyQuoteListByYearAndSave(2010, StockCategoryEnum.FinancialAndInsurance);
            return Ok(successDailyQuoteNumber2010);
        }

        public IActionResult GetDailyQuoteByDate() {
            List<DailyQuote> dailyQuoteList = DailyQuoteService.GetDailyQuoteByDate(new DateTime(2020, 5, 8));
            return Ok(dailyQuoteList);
        }

        public IActionResult GetDailyQuoteByDateAndStockCode() {
            DailyQuote dailyQuote = DailyQuoteService.GetFirstDailyQuoteByDateAndStockCode(new DateTime(2020, 5, 8), "2884");
            return Ok(dailyQuote);
        }
    }
}