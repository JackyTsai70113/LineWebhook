using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Website.Models;
using Utility;
using System.Text.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Models.Line;
using Utility.Line;
using Utility.MaskData;
using Utility.StringUtil;
using Models.Line.API;
using Utility.Google.MapAPIs;
using Models.Google.API;
using BL.Interfaces;
using BL.Services;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    public class LineWebhookController : Controller {
        public ILineWebhookService LineWebhookService { get; set; }

        public LineWebhookController() {
            LineWebhookService = new LineWebhookService();
        }

        /// <summary>
        /// LINE Webhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                //處理requestBody
                RequestBodyFromLineServer lineRequestBody = RequestHandler.GetLineRequestBody(requestBody);

                Console.WriteLine($"==========[LineWebhook/Index]==========");
                Console.WriteLine($"From LINE SERVER");
                Console.WriteLine($"body:");
                Console.WriteLine($"{JsonConvert.SerializeObject(lineRequestBody, Formatting.Indented)}");
                Console.WriteLine($"====================");

                string result = LineWebhookService.Response(lineRequestBody);
                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }
    }
}