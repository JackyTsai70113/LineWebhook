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
            //LineNotifyBotService lineNotifyBot = new LineNotifyBotService();
            Bot bot = new Bot(ConfigService.Line_ChannelAccessToken);
            //             {
            //     "type": "image",
            //     "originalContentUrl": "https://example.com/original.jpg",
            //     "previewImageUrl": "https://example.com/preview.jpg"
            // }
            //Uri uri1 = new Uri("https://i.imgur.com/Ipgt3Mz.png");

            var quickReply = new QuickReply();
            var quickReplyMessageAction = new QuickReplyMessageAction("qr", "QuickReplyButton") {
                imageUrl = new Uri("https://imgur.com/ZQVKq9T"),
            };
            quickReply.items = new List<QuickReplyItemBase>{
                quickReplyMessageAction,
                new QuickReplyPostbackAction("Buy1", "action=buy&itemid=111", "Buy2", ""),
                new QuickReplyDatetimePickerAction("Select date", "storeId=12345", DatetimePickerModes.date),
                new QuickReplyCameraAction("Open Camera"),
                new QuickReplyCamerarollAction("Open Camera roll"),
                new QuickReplyLocationAction("Location1")
            };
            var textMessage = new TextMessage("Please Select One.") {
                quickReply = quickReply
            };
            List<MessageBase> messages = new List<MessageBase>{
                textMessage
            };
            bot.PushMessage(ConfigService.Line_Jacky_userId, messages);
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