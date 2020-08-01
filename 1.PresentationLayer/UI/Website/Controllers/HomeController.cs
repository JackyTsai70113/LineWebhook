using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.MaskDatas;
using Newtonsoft.Json;
using Utility;
using Utility.Google.MapAPIs;
using Utility.MaskDatas;
using Website.Models;

namespace Website.Controllers {

    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            //var MaskDataList = MaskDataHandler.GetTopMaskDatasByComputingDistance("110台灣台北市信義區虎林街132巷37號");
            // 取得 maskData 的 List
            var MaskDataList = MaskDataSourceHandler.GetList();
            ViewData["result"] = "MaskDateList.Count: " + MaskDataList.Count.ToString();
            return View();
        }

        public IActionResult List() {
            var s = MapApiHandler.GetGeocoding("110台灣台北市信義區虎林街132巷37號");
            // 取得 maskData 的 List
            var maskDataList = MaskDataHandler.GetTopMaskDatasByComputingDistance("110台灣台北市信義區虎林街132巷37號", 5);
            StringBuilder builder = new StringBuilder();
            foreach (var maskData in maskDataList) {
                builder.AppendLine($"{maskData.Name}: 成人({maskData.AdultMasks})/兒童({maskData.ChildMasks})");
            }
            ViewData["testMaskData"] = builder.ToString();
            return View(maskDataList);
        }

        // 取得區域對應的maskDataList index
        private Dictionary<string, List<int>> GetMaskDataDict(List<MaskData> maskDataList) {
            /*

                市 City
                    區 District
                縣 County
                    鎮/鄉 Township

            */

            //Dictionary<string, int> checkFirstDict;
            var listCount = maskDataList.Count;
            var locationDict = new Dictionary<string, List<int>>();
            for (int i = 0; i < maskDataList.Count; i++) {
                var CityIndex = maskDataList[i].Address.IndexOf("市");
                var CountyIndex = maskDataList[i].Address.IndexOf("縣");
                var locationSuffix = maskDataList[i].Address.Substring(0, 6);

                if (!locationDict.ContainsKey(locationSuffix)) {
                    locationDict.Add(locationSuffix, new List<int>());
                    locationDict[locationSuffix].Add(i);
                } else {
                    locationDict[locationSuffix].Add(i);
                }
            }

            return locationDict;
        }

        private List<MaskData> GetRightList(string myLocationSuffix, Dictionary<string, List<int>> maskDataDict, List<MaskData> maskDataList) {
            var result = new List<MaskData>();
            foreach (var i in maskDataDict[myLocationSuffix]) {
                result.Add(maskDataList[i]);
            }
            return result;
        }

        public IActionResult Create() {
            MaskDealer maskDealer = new MaskDealer();
            var maskDataStringList = maskDealer.GetMaskDataResponse().Split('\n')[1];
            var maskDataArr = maskDataStringList.Split(',');
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