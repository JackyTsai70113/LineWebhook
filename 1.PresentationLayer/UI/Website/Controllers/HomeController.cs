using BL.Services;
using BL.Services.Line;
using BL.Services.MaskInstitution;
using BL.Services.TWSE_Stock;
using BL.Services.YahooFinance;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            //_logger.LogInformation("This is test.");
            LineNotifyBotService lineNotifyBot = new LineNotifyBotService();
            Bot bot = new Bot(ConfigService.Line_ChannelAccessToken);
            //             {
            //     "type": "image",
            //     "originalContentUrl": "https://example.com/original.jpg",
            //     "previewImageUrl": "https://example.com/preview.jpg"
            // }
            Uri uri = new Uri("https://drive.google.com/drive/u/0/folders/1cPTn6EF6ZXCGvbH2np2WoyM4Uw");
            Uri uri1 = new Uri("https://i.imgur.com/Ipgt3Mz.png");
            string ee1 = "https://drive.google.com/file/d/1bDGF9QgF3yb73qMJ_4PmxCwelinW2FENPQ/view?usp=sharing";
            string ee2 = "https://drive.google.com/drive/u/0/folders/1cPTn6EF6ZXCGvbH2np2WoyM4Uw";
            List<MessageBase> messages = new List<MessageBase>{
                new TextMessage("書法班開課了，快點來"),
                new ImageMessage(uri1, uri1)
            };
            string result = bot.PushMessage(ConfigService.Line_Jessi_userId, messages);
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