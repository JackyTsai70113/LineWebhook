using BL.Interfaces;
using Models.Line;
using Models.Line.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services {

    public class LineWebhookService : ILineWebhookService {

        private static string lineChannelAccessToken =
            @"tkOO80fthaESrdEWkHn5+gsypQLHd1N3DZcNsWaJku3GeO/
            HsFMyCSyU95KnA6p2bTLPFJS0y4joCknQyppqlwaDK34rrQgS
            W39EcS0j5WNEZGIlkup0nJ+xlBf+mcw89H1xKAc5Ubd0xA9/Z
            9RSIwdB04t89/1O/w1cDnyilFU=";

        private LineRequestBody LineRequestBody { get; set; }

        public string Response(LineRequestBody lineRequestBody) {
            try {
                string result;
                LineRequestBody = lineRequestBody;
                // 判斷訊息型態
                switch (LineRequestBody.Events[0].message.type.Value) {
                    case "text":
                        result = ReplyTextMessages();
                        break;

                    case "location":
                        //string result1 = ReplyLocationMessages(replyToken, handler._requestBody.Events[0].message.address);
                        result = ReplyTextMessages();
                        break;

                    default:
                        result = "";
                        break;
                }
                return result;
            } catch (Exception ex) {
                return ex.ToString();
            }
        }

        private string ReplyTextMessages() {
            string result = "";
            try {
                string url = "https://api.line.me/v2/bot/message/reply";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + lineChannelAccessToken);

                // Set up messages to send
                var messages = new List<dynamic> {
                    new StickerMessage {
                        type = "sticker",
                        packageId = "1",
                        stickerId = "1"
                    },
                    new TextMessage {
                        type = "text",
                        text = "test以下是離你最近的藥局"
                    },
                    new LocationMessage {
                        type = "location",
                        title = "myLocation",
                        address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                        latitude = 35.65910807942215,
                        longitude = 139.70372892916203
                    },
                    new LocationMessage {
                        type = "location",
                        title = "myLocation",
                        address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                        latitude = 35.65910807942215,
                        longitude = 139.70372892916203
                    }
                };

                var postData = new ReplyMessages {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                };

                // Write data to requestStream
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(JsonConvert.SerializeObject(postData));
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                //requestStream.WriteTimeout = 20000;
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result += streamReader.ReadToEnd();

                // Add Logs
                string jsonStr = JsonConvert.SerializeObject(postData, Formatting.Indented);
                Console.WriteLine($"==========[LineWebhook/ReplyMessages]==========");
                Console.WriteLine($"TO LINE SERVER: {url}");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{jsonStr}");
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            Console.WriteLine($"====================");
            return result;
        }

        private string ReplyMessages(string replyToken, List<string> messageTexts) {
            string result = "";
            try {
                string url = "https://api.line.me/v2/bot/message/reply";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + lineChannelAccessToken);

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
                messages.Add(new TextMessage {
                    type = "text",
                    text = "test以下是離你最近的藥局"
                });
                messages.Add(new LocationMessage {
                    type = "location",
                    title = "myLocation",
                    address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                    latitude = 35.65910807942215,
                    longitude = 139.70372892916203
                });
                messages.Add(new LocationMessage {
                    type = "location",
                    title = "myLocation",
                    address = "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                    latitude = 35.65910807942215,
                    longitude = 139.70372892916203
                });

                var postData = new ReplyMessages {
                    replyToken = replyToken,
                    messages = messages
                };

                // Write data to requestStream
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] data = encoding.GetBytes(JsonConvert.SerializeObject(postData));
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
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            Console.WriteLine($"====================");
            return result;
        }
    }
}