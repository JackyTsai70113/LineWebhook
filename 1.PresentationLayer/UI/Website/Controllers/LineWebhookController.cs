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
            Console.WriteLine($"before requestBody:");
            Console.WriteLine($"{requestBody}");
            Console.WriteLine($"after requestBody:");
            string jsonString = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
            Console.WriteLine($"{jsonString}");
            Console.WriteLine($"====================");
            
            LineSource lineSource = JsonConvert.DeserializeObject<LineSource>(requestBody.ToString());
            string replyToken = lineSource.events[0].replyToken;
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
                var messages = new List<TextMessage>();
                foreach(var text in messageTexts)
                {
                    messages.Add(new TextMessage
                    {
                        type = "text",
                        text = text
                    });
                }

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

        [HttpPost]
        public IActionResult MessageHandler([FromBody] Event myEvent)
        {
            string jsonString = JsonConvert.SerializeObject(myEvent, Formatting.Indented);
            return Content(jsonString);
        }
    }
}