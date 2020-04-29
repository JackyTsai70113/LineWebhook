using BL.Interfaces;
using Models.Google.API;
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
using Utility.Google.MapAPIs;
using Utility.Line;
using Utility.MaskData;
using Utility.StringUtil;

namespace BL.Services {

    public class LineWebhookService : ILineWebhookService {

        private static string lineChannelAccessToken =
            @"tkOO80fthaESrdEWkHn5+gsypQLHd1N3DZcNsWaJku3GeO/
            HsFMyCSyU95KnA6p2bTLPFJS0y4joCknQyppqlwaDK34rrQgS
            W39EcS0j5WNEZGIlkup0nJ+xlBf+mcw89H1xKAc5Ubd0xA9/Z
            9RSIwdB04t89/1O/w1cDnyilFU=";

        private RequestBodyFromLineServer LineRequestBody { get; set; }

        /// <summary>
        /// 判讀LineServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBodyFromLineServer">LineServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response(RequestBodyFromLineServer requestBodyFromLineServer) {
            try {
                string result;
                LineRequestBody = requestBodyFromLineServer;
                // 判斷訊息型態
                switch (LineRequestBody.Events[0].message.type) {
                    case "text":
                        result = ReplyTestMessages("text");
                        break;

                    case "sticker":
                        //string result1 = ReplyLocationMessages(replyToken, handler._requestBody.Events[0].message.address);
                        result = ReplyTestMessages("sticker");
                        break;

                    case "location":
                        //string result1 = ReplyLocationMessages(replyToken, handler._requestBody.Events[0].message.address);
                        string address = LineRequestBody.Events[0].message.address;
                        result = ReplyPharmacyInfo(address);
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

        /// <summary>
        /// 根據指定字串進行回應
        /// </summary>
        /// <param name="text">指定字串</param>
        /// <returns>LOG紀錄</returns>
        private string ReplyTestMessages(string text) {
            string result = "";
            try {
                // Set up messages to send
                List<dynamic> messages = new List<dynamic> {
                    new StickerMessage {
                        type = "sticker",
                        packageId = "1",
                        stickerId = "8"
                    },
                    new TextMessage {
                        type = "text",
                        text = "訊息類型: " + text
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

                result = ResponseHandler.PostToLineServer(new RequestBodyToLine {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// 根據指定地址進行回應藥局資訊
        /// </summary>
        /// <param name="address">指定地址</param>
        /// <returns>LOG紀錄</returns>
        private string ReplyPharmacyInfo(string address) {
            string result = "";
            try {
                // 取得欲傳送的MaskDataList
                var topMaskDatas = MaskDataHandler.GetTopMaskDatasByComputingDistance(address, 5);

                // Set up messages to send
                var messages = new List<dynamic>();
                StringBuilder builder = new StringBuilder();

                if (topMaskDatas.Count == 0) {
                    string locationSuffix = LocationHandler.GetLocationFirstDivisionSuffix(address);
                    builder.Append($"所在區域({locationSuffix})沒有相關藥局");
                    messages.Add(new TextMessage {
                        type = "text",
                        text = builder.ToString()
                    });
                } else {
                    foreach (var maskData in topMaskDatas) {
                        Location location = MapApiHandler.GetGeocoding(maskData.Address).results[0].geometry.location;

                        if (!Double.TryParse(location.lat, out double lat)) {
                            Console.WriteLine($"Ex: Cannot parse {location.lat} to Int.");
                            lat = Double.MinValue;
                        }

                        if (!Double.TryParse(location.lng, out double lng)) {
                            Console.WriteLine($"Ex: Cannot parse {location.lng} to Int.");
                            lng = Double.MinValue;
                        }

                        messages.Add(new LocationMessage {
                            type = "location",
                            title = maskData.Name + "\n"
                                + "(成人:" + maskData.AdultMasks
                                + "/兒童: " + maskData.ChildMasks + ")",
                            address = maskData.Address,
                            latitude = lat,
                            longitude = lng
                        });
                    }
                }

                result = ResponseHandler.PostToLineServer(new RequestBodyToLine {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }
    }
}