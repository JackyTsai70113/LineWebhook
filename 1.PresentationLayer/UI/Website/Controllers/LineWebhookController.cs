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
using Utility.MaskDataHandler;
using Utility.StringUtil;
using Models.Line.API;
using Utility.Google.MapAPIs;
using Models.Google.API;

namespace Website.Controllers
{
    public class LineWebhookController : Controller
    {
        private static string channelAccessToken = 
            @"tkOO80fthaESrdEWkHn5+gsypQLHd1N3DZcNsWaJku3GeO/
            HsFMyCSyU95KnA6p2bTLPFJS0y4joCknQyppqlwaDK34rrQgS
            W39EcS0j5WNEZGIlkup0nJ+xlBf+mcw89H1xKAc5Ubd0xA9/Z
            9RSIwdB04t89/1O/w1cDnyilFU=";
        
        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody)
        {
            Console.WriteLine($"==========[LineWebhook/Index]==========");
            Console.WriteLine($"From LINE SERVER");
            Console.WriteLine($"requestBody:");
            Console.WriteLine($"{requestBody}");
            Console.WriteLine($"====================");
            
            RequestHandler handler = new RequestHandler(requestBody);

            LineRequestBody body = handler.requestBody;
            string replyToken = body.events[0].replyToken;
            switch(handler.messageType)
            {
                case "text":
                    return Content("");
                case "location":
                    //Models.Line.API.LocationMessage locationMsg = (LocationMessage)(body.events[0].message);
                    string result1 = ReplyLocationMessages(replyToken, body.events[0].message.address);
                    return Content(requestBody.ToString() + "\n" + result1);
            }
            //LineRequestBody body = JsonConvert.DeserializeObject<LineRequestBody>(requestBody.ToString());
            //string replyToken2 = lineSource.events[0].message.text;
            List<string> messageTexts = new List<string>();
            messageTexts.Add("修但幾勒");
            string result = ReplyMessages(replyToken, messageTexts);
            return Content(requestBody.ToString() + "\n" + result);
        }

        private string ReplyMessages(string replyToken, List<string> messageTexts)
        {
            string result = "";
            try
            {
                string url = "https://api.line.me/v2/bot/message/reply";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + channelAccessToken);

                // Set up messages to send
                var messages = new List<dynamic>();
                // foreach(var text in messageTexts)
                // {
                //     messages.Add(new TextMessage
                //     {
                //         type = "text",
                //         text = text
                //     });
                // }

                // messages.Add(new StickerMessage
                // {
                //     type = "sticker",
                //     packageId = "1",
                //     stickerId = "1"
                // });
                messages.Add(new TextMessage
                {
                    type = "text",
                    text = "test以下是離你最近的藥局"
                });
                messages.Add(new LocationMessage
                {
                    type = "location",
                    title = "myLocation",
                    address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                    latitude = 35.65910807942215,
                    longitude = 139.70372892916203
                });
                messages.Add(new LocationMessage
                { 
                    type = "location",
                    title = "myLocation",
                    address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                    latitude = 35.65910807942215,
                    longitude = 139.70372892916203
                });

                var postData = new ReplyMessages{
                    replyToken = replyToken,
                    messages = messages
                };

                // Write data to requestStream
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] data = encoding.GetBytes(System.Text.Json.JsonSerializer.Serialize(postData));
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                //requestStream.WriteTimeout = 20000;
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                var response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result += streamReader.ReadToEnd();

                // Add Logs
                string jsonStr = JsonConvert.SerializeObject(postData, Formatting.Indented);
                Console.WriteLine($"==========[LineWebhook/ReplyMessages]==========");
                Console.WriteLine($"TO LINE SERVER: {url}");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{jsonStr}");
            }
            catch(Exception ex)
            {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            Console.WriteLine($"====================");
            return result;
        }
        private string ReplyLocationMessages(string replyToken, string address)
        {
            string result = "";
            try
            {
                // 取得欲傳送的MaskDataList
                var topMaskDatas = MaskDataHandler.GetTopMaskDatasByComputingDistance(address, 5);
                
                // create Request
                string url = "https://api.line.me/v2/bot/message/reply";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + channelAccessToken);


                // Set up messages to send
                var messages = new List<dynamic>();
                StringBuilder builder = new StringBuilder();
                
               
                
                if(topMaskDatas.Count == 0)
                {
                    string locationSuffix = LocationHandler.GetLocationFirstDivisionSuffix(address);
                    builder.Append($"所在區域({locationSuffix})沒有相關藥局");
                    messages.Add(new TextMessage
                    {
                        type = "text",
                        text = builder.ToString()
                    });
                }
                else
                {
                    foreach (var maskData in topMaskDatas)
                    {
                        Location location = MapApiHandler.GetGeocoding(maskData.Address).results[0].geometry.location;

                        if (!Double.TryParse(location.lat, out double lat))
                        {
                            Console.WriteLine($"Ex: Cannot parse {location.lat} to Int.");
                            lat = Double.MinValue;
                        }

                        if (!Double.TryParse(location.lng, out double lng))
                        {
                            Console.WriteLine($"Ex: Cannot parse {location.lng} to Int.");
                            lng = Double.MinValue;
                        }

                        messages.Add(new LocationMessage
                        { 
                            type = "location",
                            title = maskData.Name,
                            address = maskData.Address,
                            latitude = lat,
                            longitude = lng
                        });
                        // builder.Append(maskData.Name + " " + maskData.Address + " " + 
                        //     maskData.AdultMasks + " " + maskData.ChildMasks + "\n");
                    }
                }
               
                // messages.Add(new LocationMessage
                // {
                //     type = "location",
                //     title = "myLocation",
                //     address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                //     latitude = 35.65910807942215,
                //     longitude = 139.70372892916203
                // });
                

                var postData = new ReplyMessages{
                    replyToken = replyToken,
                    messages = messages
                };

                // Add Logs
                string jsonStr = JsonConvert.SerializeObject(postData, Formatting.Indented);
                Console.WriteLine($"==========[LineWebhook/ReplyMessages]==========");
                Console.WriteLine($"TO LINE SERVER: {url}");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{jsonStr}");

                // Write data to requestStream
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] data = encoding.GetBytes(System.Text.Json.JsonSerializer.Serialize(postData));
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                //requestStream.WriteTimeout = 20000;
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                var response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result += streamReader.ReadToEnd();
            }
            catch(Exception ex)
            {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            Console.WriteLine($"====================");
            return result;
        }

        // [HttpPost]
        // public IActionResult MessageHandler([FromBody] Event myEvent)
        // {
        //     string jsonString = JsonConvert.SerializeObject(myEvent, Formatting.Indented);
        //     return Content(jsonString);
        // }
    }
}