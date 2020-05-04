using AngleSharp;
using AngleSharp.Dom;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Transactions;

namespace DA.Managers.TWSE_Stock {

    public class CashDividendManager {

        public static List<DividendDistribution> Get(Array stockCodeEnums) {
            List<DividendDistribution> dividendDistributions = new List<DividendDistribution>();
            foreach (StockCodeEnum stockCodeEnum in stockCodeEnums) {
                dividendDistributions.Add(Get(stockCodeEnum));
            }
            return dividendDistributions;
        }

        public static DividendDistribution Get(StockCodeEnum stockCodeEnum) {
            string stockCodeStr = ((int)stockCodeEnum).ToString();
            string uri = $"https://mops.twse.com.tw/mops/web/ajax_t05st09_2?encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&inpuType=co_id&TYPEK=all&isnew=false" +
                $"&co_id=" + stockCodeStr +
                $"&date1=99&date2=109&qryType=1";

            HttpClient client = new HttpClient();
            HttpContent httpContent = new StringContent("");
            //HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
            //發送請求
            HttpResponseMessage httpResponseMessage = client.PostAsync(uri, httpContent).Result;

            //檢查回應的伺服器狀態StatusCode是否是200 OK
            if (httpResponseMessage.StatusCode != HttpStatusCode.OK) {
                return null;
            }
            string contentStr = httpResponseMessage.Content.ReadAsStringAsync().Result;//取得內容

            // 當年年份
            int nowYear = DateTimeUtility.GetNowYear();

            // Angle Sharp Setting
            IConfiguration confiuration = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(confiuration);

            IDocument document = context.OpenAsync(req => req.Content(contentStr)).Result;
            IElement table = document.QuerySelectorAll("table")[2];
            IHtmlCollection<IElement> trElements = table.QuerySelectorAll("tr");

            List<float> cashDividendsIn10Year = new List<float>();
            List<float> stockDividendsIn10Year = new List<float>();
            // 3為實際tr開始index，10為計算區間
            for (int i = 3; i < trElements.Length || i < 10 + 3; i++) {
                IHtmlCollection<IElement> tdElements = trElements[i].QuerySelectorAll("td");
                string yearStr = tdElements[1].InnerHtml.StripHtmlSpace().StripHtmlTag();
                int year = int.Parse(yearStr.TrimEnd("年")).GetADYear();
                // 取10年股利
                if (nowYear - year <= 10) {
                    float cashDividend = tdElements[10].InnerHtml.StripHtmlSpace().ToFloat() + tdElements[11].InnerHtml.StripHtmlSpace().ToFloat();
                    cashDividendsIn10Year.Add(cashDividend);

                    float stockDividend = float.Parse(tdElements[13].InnerHtml.StripHtmlSpace());
                    stockDividendsIn10Year.Add(stockDividend);
                }
            }
            float recentDividend = cashDividendsIn10Year.FirstOrDefault() + stockDividendsIn10Year.FirstOrDefault();
            float dividendIn5Years = cashDividendsIn10Year.Take(5).Average() + stockDividendsIn10Year.Take(5).Average();
            float dividendIn10Years = cashDividendsIn10Year.Average() + stockDividendsIn10Year.Average();
            DividendDistribution dividendDistribution = new DividendDistribution {
                StockCode = stockCodeEnum,
                RecentDividend = recentDividend,
                DividendIn5Years = dividendIn5Years,
                DividendIn10Years = dividendIn10Years,
                RecentCheapPrice = recentDividend * 15,
                RecentReasonablePrice = recentDividend * 20,
                RecentExpensivePrice = recentDividend * 30,
                CheapPriceIn10Years = dividendIn10Years * 15,
                ReasonablePriceIn10Years = dividendIn10Years * 20,
                ExpensivePriceIn10Years = dividendIn10Years * 30
            };

            return dividendDistribution;
        }
    }
}