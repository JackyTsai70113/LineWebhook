using AngleSharp;
using AngleSharp.Dom;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using DA.Managers.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DA.Managers.TWSE_Stock {

    public class DividendDistributionManager : IDividendDistributionManager {

        public List<DividendDistribution> CrawlDividendDistribution(StockCodeEnum stockCodeEnum) {
            List<DividendDistribution> result = CrawlDividendDistributionListIn10YearsAsync(stockCodeEnum).Result;
            return result;
        }

        public List<DividendDistribution> CrawlDividendDistribution(StockCodeEnum[] stockCodeEnums) {
            List<DividendDistribution> result = new List<DividendDistribution>();

            var taskList = new List<Task<List<DividendDistribution>>>();
            foreach (StockCodeEnum stockCodeEnum in stockCodeEnums) {
                taskList.Add(CrawlDividendDistributionListIn10YearsAsync(stockCodeEnum));
            }
            List<DividendDistribution>[] dividendDistributionListList = Task.WhenAll(taskList).Result;
            foreach (var dividendDistributionList in dividendDistributionListList) {
                result.AddRange(dividendDistributionList);
            }

            return result;
        }

        //public DividendDistribution Get(StockCodeEnum stockCodeEnum) {
        //    string stockCodeStr = ((int)stockCodeEnum).ToString();
        //    string url = $"https://mops.twse.com.tw/mops/web/ajax_t05st09_2?encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&inpuType=co_id&TYPEK=all&isnew=false" +
        //        $"&co_id=" + stockCodeStr +
        //        $"&date1=99&date2=109&qryType=1";

        //    string contentStr = RequestUtility.GetContentFromGetRequest(url);

        //    // 當年年份
        //    int nowYear = DateTimeUtility.NowYear;

        //    // Angle Sharp Setting
        //    IConfiguration configuration = Configuration.Default;
        //    IBrowsingContext context = BrowsingContext.New(configuration);

        //    IDocument document = context.OpenAsync(req => req.Content(contentStr)).Result;
        //    //從第3個Table開始爬
        //    IElement table = document.QuerySelectorAll("table")[2];
        //    IHtmlCollection<IElement> trElements = table.QuerySelectorAll("tr");

        //    List<float> cashDividendsIn10Year = new List<float>();
        //    List<float> stockDividendsIn10Year = new List<float>();
        //    // 從 tr 為 index=3 的開始爬，10為計算區間
        //    for (int i = 3; i < trElements.Length || i < 10 + 3; i++) {
        //        IHtmlCollection<IElement> tdElements = trElements[i].QuerySelectorAll("td");
        //        string yearStr = tdElements[1].InnerHtml.StripHtmlSpace().StripHtmlTag();
        //        int year = int.Parse(yearStr.TrimEnd("年")).GetADYear();
        //        // 取10年股利
        //        if (nowYear - year <= 10) {
        //            float cashDividend = tdElements[10].InnerHtml.StripHtmlSpace().ToFloat() + tdElements[11].InnerHtml.StripHtmlSpace().ToFloat();
        //            cashDividendsIn10Year.Add(cashDividend);

        //            float stockDividend = float.Parse(tdElements[13].InnerHtml.StripHtmlSpace());
        //            stockDividendsIn10Year.Add(stockDividend);
        //        }
        //    }
        //    float recentDividend = cashDividendsIn10Year.FirstOrDefault() + stockDividendsIn10Year.FirstOrDefault();
        //    float dividendIn5Years = cashDividendsIn10Year.Take(5).Average() + stockDividendsIn10Year.Take(5).Average();
        //    float dividendIn10Years = cashDividendsIn10Year.Average() + stockDividendsIn10Year.Average();
        //    DividendDistribution dividendDistribution = new DividendDistribution {
        //        StockCode = stockCodeEnum,
        //        RecentDividend = recentDividend,
        //        DividendIn5Years = dividendIn5Years,
        //        DividendIn10Years = dividendIn10Years,
        //        RecentCheapPrice = recentDividend * 15,
        //        RecentReasonablePrice = recentDividend * 20,
        //        RecentExpensivePrice = recentDividend * 30,
        //        CheapPriceIn10Years = dividendIn10Years * 15,
        //        ReasonablePriceIn10Years = dividendIn10Years * 20,
        //        ExpensivePriceIn10Years = dividendIn10Years * 30
        //    };

        //    return dividendDistribution;
        //}

        private async Task<List<DividendDistribution>> CrawlDividendDistributionListIn10YearsAsync(StockCodeEnum stockCodeEnum) {
            //股票代號字串
            string stockCodeStr = ((int)stockCodeEnum).ToString();

            //當前民國年份
            int nowROCYear = DateTimeUtility.NowROCYear;
            string nowROCYearStr = nowROCYear.ToString();
            //十年前民國年份
            string ROCYear10YearsAgoStr = (nowROCYear - 10).ToString();
            string url = $"https://mops.twse.com.tw/mops/web/ajax_t05st09_2?encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&inpuType=co_id&TYPEK=all&isnew=false" +
                $"&co_id=" + stockCodeStr +
                $"&date1=" + ROCYear10YearsAgoStr +
                $"&date2=" + nowROCYear +
                $"&qryType=1";

            Stream stream = await RequestUtility.GetStreamFromGetRequestAsync(url);
            // Angle Sharp Setting
            IConfiguration configuration = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(configuration);

            IDocument document = await context.OpenAsync(req => req.Content(stream));
            //從第3個Table開始爬
            IElement table;
            try {
                table = document.QuerySelectorAll("table")[2];
            } catch (Exception ex) {
                stream = await RequestUtility.GetStreamFromGetRequestAsync(url);
                document = await context.OpenAsync(req => req.Content(stream));
                table = document.QuerySelectorAll("table")[2];
            }
            IHtmlCollection<IElement> trElements = table.QuerySelectorAll("tr");

            //List<float> cashDividendsIn10Year = new List<float>();
            //List<float> stockDividendsIn10Year = new List<float>();
            List<DividendDistribution> dividendDistributionList = new List<DividendDistribution>();
            // 3: 從 tr 為 index=3 的開始爬，10: 計算10年就好
            for (int i = 3; i < trElements.Length && i < 10 + 3; i++) {
                IHtmlCollection<IElement> tdElements = trElements[i].QuerySelectorAll("td");
                string yearStr = tdElements[1].InnerHtml.StripHtmlSpace().StripHtmlTag();
                int year = 0;
                if (yearStr.IndexOf('上') != -1) {
                    year = int.Parse(yearStr.TrimEnd("年上半年"));
                } else if (yearStr.IndexOf('下') != -1) {
                    year = int.Parse(yearStr.TrimEnd("年下半年"));
                } else {
                    year = int.Parse(yearStr.TrimEnd("年年度"));
                }
                DividendDistribution dividendDistribution = new DividendDistribution {
                    StockCode = stockCodeEnum.ToStockCode(),
                    Year = (short)year.ToADYear(),
                    CashDividendsToBeDistributedFromRetainedEarnings = tdElements[10].InnerHtml.StripHtmlSpace().ToFloat(),
                    CashDividendsFromLegalReserveAndCapitalSurplus = tdElements[11].InnerHtml.StripHtmlSpace().ToFloat(),
                    SharesDistributedFromEarnings = tdElements[13].InnerHtml.StripHtmlSpace().ToFloat(),
                    SharesDistributedFromLegalReserveAndCapitalSurplus = tdElements[14].InnerHtml.StripHtmlSpace().ToFloat()
                };
                // 取10年股利
                //if (nowROCYear - year <= 10) {
                //    float cashDividend = tdElements[10].InnerHtml.StripHtmlSpace().ToFloat() + tdElements[11].InnerHtml.StripHtmlSpace().ToFloat();
                //    cashDividendsIn10Year.Add(cashDividend);

                //    float stockDividend = float.Parse(tdElements[13].InnerHtml.StripHtmlSpace());
                //    stockDividendsIn10Year.Add(stockDividend);
                //}
                dividendDistributionList.Add(dividendDistribution);
            }
            //float recentDividend = cashDividendsIn10Year.FirstOrDefault() + stockDividendsIn10Year.FirstOrDefault();
            //float dividendIn5Years = cashDividendsIn10Year.Take(5).Average() + stockDividendsIn10Year.Take(5).Average();
            //float dividendIn10Years = cashDividendsIn10Year.Average() + stockDividendsIn10Year.Average();
            //DividendDistribution dividendDistribution = new DividendDistribution {
            //    StockCode = stockCodeEnum,
            //    RecentDividend = recentDividend,
            //    DividendIn5Years = dividendIn5Years,
            //    DividendIn10Years = dividendIn10Years,
            //    RecentCheapPrice = recentDividend * 15,
            //    RecentReasonablePrice = recentDividend * 20,
            //    RecentExpensivePrice = recentDividend * 30,
            //    CheapPriceIn10Years = dividendIn10Years * 15,
            //    ReasonablePriceIn10Years = dividendIn10Years * 20,
            //    ExpensivePriceIn10Years = dividendIn10Years * 30
            //};

            return dividendDistributionList;
        }
    }
}