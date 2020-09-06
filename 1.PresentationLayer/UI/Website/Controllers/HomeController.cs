using BL.Services.MaskInstitution;
using BL.Services.TWSE_Stock;
using BL.Services.YahooFinance;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Website.Models;

namespace Website.Controllers {

    public class HomeController : Controller {
        private readonly MaskInstitutionService _maskInstitutionService;

        public HomeController() {
            _maskInstitutionService = new MaskInstitutionService();
        }

        public IActionResult Test() {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("\n");
            //for (int i = 0; i < 50; i++) {
            //    sb.Append("證券名稱 " + i + "00000000\n");
            //}
            //string text = sb.ToString();
            //new LineNotifyBotService().PushMessage_Group(text);
            //new TradingVolumeService();
            //new LineWebhookService("");
            new TradingVolumeService();
            return new OkResult();
        }

        public IActionResult Index() {
            int maskInstitutionCount = _maskInstitutionService.GetCount();
            ViewData["result"] = "MaskDateList.Count: " + maskInstitutionCount;
            return View();
        }

        public IActionResult List() {
            // 取得 maskData 的 List
            var maskDataList = _maskInstitutionService.GetTopMaskDatasByComputingDistance("110台灣台北市信義區虎林街132巷37號", 5);
            StringBuilder builder = new StringBuilder();
            foreach (var maskData in maskDataList) {
                builder.AppendLine($"{maskData.Name}: 成人({maskData.AdultMasks})/兒童({maskData.ChildMasks})");
            }
            ViewData["testMaskData"] = builder.ToString();
            return View(maskDataList);
        }

        public IActionResult Create() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        public IActionResult KD() {
            StockService stockService = new StockService();
            List<object[]> subCandles = stockService.GetSubCandles();

            return View(subCandles);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}