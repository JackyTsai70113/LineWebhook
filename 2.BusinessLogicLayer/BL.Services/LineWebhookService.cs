﻿using BL.Interfaces;
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
using Utility.MaskDatas;
using Utility.StringUtil;

namespace BL.Services {

    public class LineWebhookService : ILineWebhookService {
        private RequestBodyFromLineServer LineRequestBody { get; set; }

        /// <summary>
        /// 判讀LineServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">LineServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response(dynamic requestBody) {
            try {
                string result;

                //處理requestBody
                RequestBodyFromLineServer lineRequestBody = RequestHandler.GetLineRequestBody(requestBody);

                Console.WriteLine($"==========[LineWebhookService/requestBody]==========");
                Console.WriteLine($"From LINE SERVER");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{JsonConvert.SerializeObject(lineRequestBody, Formatting.Indented)}");
                Console.WriteLine($"====================");

                LineRequestBody = lineRequestBody;
                // 判斷訊息型態
                dynamic message = LineRequestBody.Events[0].message;
                switch (message.type) {
                    case "text":
                        result = ReplyTestMessages(message.type);
                        break;

                    case "location":
                        string address = message.address;
                        result = ReplyPharmacyInfo(address);
                        break;

                    case "sticker":
                        result = ReplyStickerMessages((StickerMessage)message);
                        break;

                    default:
                        Console.WriteLine($"無相符的 message.type: {message.type}, requestBodyFromLineServer: " +
                            $"{JsonConvert.SerializeObject(requestBody, Formatting.Indented)}");
                        result = ReplyTestMessages(message.type);
                        break;
                }
                return result;
            } catch (Exception ex) {
                Console.WriteLine($"LineWebhookService.Response 錯誤, requestBody:{requestBody}");
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
                                + "成人: " + maskData.AdultMasks + "\n"
                                + "兒童: " + maskData.ChildMasks,
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

        private string ReplyStickerMessages(StickerMessage stickerMessage) {
            string result = "";
            try {
                // Set up messages to send
                List<dynamic> messages = new List<dynamic> {
                    new StickerMessage {
                        type = "sticker",
                        packageId = stickerMessage.packageId,
                        stickerId = stickerMessage.stickerId + 1
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = stickerMessage.packageId,
                        stickerId = stickerMessage.stickerId + 2
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = stickerMessage.packageId,
                        stickerId = stickerMessage.stickerId + 3
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = stickerMessage.packageId,
                        stickerId = stickerMessage.stickerId + 4
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = stickerMessage.packageId,
                        stickerId = stickerMessage.stickerId + 5
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
    }
}