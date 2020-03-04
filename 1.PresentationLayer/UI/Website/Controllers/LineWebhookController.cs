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

namespace Website.Controllers
{
    public class LineWebhookController : Controller
    {
        [HttpPost]
        public IActionResult Index([FromBody] LineSource lineSource)
        {
            string jsonString = JsonSerializer.Serialize(lineSource);
            string replyToken = lineSource.events[0].replyToken;
            string result = PostTo(replyToken);
            return Content(jsonString + "\n" + result);
        }

        private string PostTo(string replyToken)
        {
            string result = "";
            try
            {
                string channelAccessToken = "tkOO80fthaESrdEWkHn5+gsypQLHd1N3DZcNsWaJku3GeO/HsFMyCSyU95KnA6p2bTLPFJS0y4joCknQyppqlwaDK34rrQgSW39EcS0j5WNEZGIlkup0nJ+xlBf+mcw89H1xKAc5Ubd0xA9/Z9RSIwdB04t89/1O/w1cDnyilFU=";
                string url = "https://api.line.me/v2/bot/message/reply";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + channelAccessToken);

                var postData = new ReplyMessages{
                    replyToken = replyToken,
                    messages = new List<Message>{
                        new Message{
                            type = "text",
                            id = "",
                            text = "Hello, user(" + replyToken + ")"
                        },
                        new Message{
                            type = "text",
                            id = "",
                            text = "May I help you?"
                        }
                    }
                };

                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] data = encoding.GetBytes(JsonSerializer.Serialize(postData));
                request.ContentLength = data.Length;

                Stream sendStream = request.GetRequestStream();
                //sendStream.WriteTimeout = 20000;
                sendStream.Write(data, 0, data.Length);
                sendStream.Close();

                var response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result += streamReader.ReadToEnd();

                Console.WriteLine("Result: " + result);
                Console.WriteLine("我在這!");
            }
            catch(Exception ex)
            {
                result += "Exception: " + ex.Message;
            }
            return result;
        }

        [HttpPost]
        public IActionResult MessageHandler([FromBody] Event myEvent)
        {
            string jsonString = JsonSerializer.Serialize(myEvent);
            return Content(jsonString);
        }
    }
}