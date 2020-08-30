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
            //    "fix verb uk /fɪks/ us /fɪks/\n\nto repair something\n修理\n------------------------\nThey couldn't fix my old computer, so"
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