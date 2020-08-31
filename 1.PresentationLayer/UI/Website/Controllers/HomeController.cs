using BL.Services.MaskInstitution;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Website.Models;
using Website.Services;

namespace Website.Controllers {

    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly MaskInstitutionService _maskInstitutionService;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
            _maskInstitutionService = new MaskInstitutionService();
        }

        public IActionResult Test() {
            ////傳送對象
            //var toUserID = ConfigService.LineJ_userId;
            ////Channel Access Token
            //var token = ConfigService.LineChannelAccessToken;
            ////create bot instance
            //Bot bot = new Bot(token);
            ////send message
            //bot.PushMessage(toUserID,
            //    "fit adjective uk /fɪt/ us /fɪt/\n\nhealthy and strong, especially as a result of exercise\n健壯的，健康的\n------------------------\nI jog to keep fit.\n我透過慢跑來健身。\nYou need to be very fit to hike the Inca Trail.\n徒步印加古道必須具備強壯的體魄。\n\nsuitable for a particular purpose or activity\n 合適的；相稱的\n------------------------\nShe's not fit for the level of responsibility she's been given.\n���挑不起肩上的那個重擔。\n\nto not be able to do something because you are upset, ill, drunk, etc.\n不宜做…；不在做…的狀態\n------------------------\nHe's very upset and is in no fit state to drive.\n他心煩意亂，不宜駕車。\n\nsafe for people to eat\n可供人食用\n\nSomething that is fit for purpose does what it is meant to do.\n十分適合用來做某事的；專門用於做某事的\n\nto consider an action or decision to be correct for the situation\n認為（行動或決定）恰當（或合適）\n------------------------\nJust do whatever you think fit - I'm sure you'll make the right decision.\n你覺得怎樣合適就怎樣做吧，我相信你會作出正確決定的。\nSpend the money as you see fit.\n把錢花在你認為該花的地方。\n\nsexually attractive\n性感的\n------------------------\nI met this really fit bloke in a club last night.\n昨晚，我在俱樂部遇到了這個十分性感的小子。\n"
            //);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult postToLine([FromBody] dynamic body) {
            PostData postDataObj = JsonConvert.DeserializeObject<PostData>(body.ToString());
            var uri = postDataObj.uri;
            var postData = postDataObj.postData;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // 設定Request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Content-Type", "application/json");

            // 將postData寫入
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] buffer = encoding.GetBytes(System.Text.Json.JsonSerializer.Serialize(postData));
            httpWebRequest.ContentLength = buffer.Length;
            var requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            // 取得回應並記錄
            var response = httpWebRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var streamReader = new StreamReader(responseStream);

            var responseStr = streamReader.ReadToEnd();
            return Content(responseStr);
        }
    }
}