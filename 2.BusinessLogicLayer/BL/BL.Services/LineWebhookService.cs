using BL.Interfaces;
using Core.Domain.DTO.ResponseDTO.Line;
using Core.Domain.DTO.ResponseDTO.Line.Messages;
using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates;
using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using Models.Google.API;
using Models.Line;

//using Models.Line.API;

using Models.Line.Webhook;
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

        public LineWebhookService() {
            CambridgeDictionaryManager = new CambridgeDictionaryManager();
        }

        private RequestBodyFromLineServer LineRequestBody { get; set; }
        private ICambridgeDictionaryManager CambridgeDictionaryManager { get; set; }

        /// <summary>
        /// 判讀LineServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">LineServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response(dynamic requestBody) {
            string result = "";
            try {
                //處理requestBody
                RequestBodyFromLineServer lineRequestBody = RequestHandler.GetLineRequestBody(requestBody);

                Console.WriteLine($"========== From LINE SERVER ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{JsonConvert.SerializeObject(lineRequestBody, Formatting.Indented)}");
                Console.WriteLine($"====================");

                LineRequestBody = lineRequestBody;
                // 判斷訊息型態
                dynamic message = LineRequestBody.Events[0].message;
                switch ((string)message.type) {
                    case "text":
                        result = ReplyTextMessages((string)message.text);
                        break;

                    case "location":
                        string address = message.address;
                        result = ReplyPharmacyInfo(address);
                        break;

                    case "sticker":
                        result = ReplyStickerMessages();
                        break;

                    default:
                        Console.WriteLine($"無相符的 message.type: {(string)message.type}, requestBodyFromLineServer: " +
                            $"{JsonConvert.SerializeObject(requestBody, Formatting.Indented)}");
                        result += ReplyTestMessages((string)message.type);
                        break;
                }
                return result;
            } catch (Exception ex) {
                Console.WriteLine(
                    $"LineWebhookService.Response 錯誤, " +
                    $"requestBody:{ requestBody}" +
                    $"result: {result}" +
                    $"ex: {ex}");
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
                    new TextMessage {
                        type = "text",
                        text = text
                    }
                };

                result = ResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private string ReplyTextMessages(string text) {
            string result = "";
            try {
                // Set up messages to send
                if (text.StartsWith("sticker")) {
                    string packageIdStr = text.Split(' ')[1];
                    string stickerIdStr = text.Split(' ')[2];
                    result = ReplyStickerMessages(packageIdStr, stickerIdStr);
                } else if (text.StartsWith("cd ")) {
                    string vocabulary = text.Split(' ')[1];
                    result = ReplyCambridgeDictionaryMessages(vocabulary);
                } else {
                    result = ReplyTestMessages(text);
                }
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex}");
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

                result = ResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private string ReplyStickerMessages(string packageIdStr = "1", string stickerIdStr = "1") {
            string result = "";
            try {
                if (!Int32.TryParse(packageIdStr, out int packageId)) {
                    Console.WriteLine($"Ex: Cannot parse {packageIdStr} to Int.");
                    packageId = 1;
                }

                if (!Int32.TryParse(stickerIdStr, out int stickerId)) {
                    Console.WriteLine($"Ex: Cannot parse {stickerIdStr} to Int.");
                    stickerId = 1;
                }

                // Set up messages to send
                List<dynamic> messages = new List<dynamic> {
                    new StickerMessage {
                        type = "sticker",
                        packageId = packageId.ToString(),
                        stickerId = (stickerId++).ToString()
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = packageId.ToString(),
                        stickerId = (stickerId++).ToString()
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = packageId.ToString(),
                        stickerId = (stickerId++).ToString()
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = packageId.ToString(),
                        stickerId = (stickerId++).ToString()
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = packageId.ToString(),
                        stickerId = (stickerId++).ToString()
                    }
                };

                result = ResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private string ReplyCambridgeDictionaryMessages(string vocabulary) {
            string result = "";
            try {
                // Set up messages to send
                List<dynamic> messages = new List<dynamic> {
                    new TemplateMessage() {
                        altText = "this is a carousel template",
                        template = new CarouselTemplate() {
                            columns = new List<ColumnObject>() {
                                new ColumnObject() {
                                    thumbnailImageUrl = "https://example.com/bot/images/item1.jpg",
                                    imageBackgroundColor = "#FFFFFF",
                                    title = "this is menu",
                                    text = "description",
                                    defaultAction = new MessageAction() {
                                        type = "uri",
                                        label = "View detail",
                                        text = "message action text"
                                    }
                                }
                            },
                            imageAspectRatio = "rectangle",
                            imageSize = "cover"
                        }
                    }
                };

                result = ResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = LineRequestBody.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private List<StickerMessage> GetStickerMessages(string packageId = "0", string stickerId = "0") {
            List<StickerMessage> stickerMessages = new List<StickerMessage>();
            try {
                // Set up messages to send
                stickerMessages = new List<StickerMessage> {
                    new StickerMessage {
                        type = "sticker",
                        packageId = "1",
                        stickerId = "8"
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = "1",
                        stickerId = "9"
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = "1",
                        stickerId = "10"
                    },
                    new StickerMessage {
                        type = "sticker",
                        packageId = "1",
                        stickerId = "11"
                    }
                };
            } catch (Exception ex) {
                Console.WriteLine($"packageId: {packageId} stickerId: {stickerId}");
                Console.WriteLine($"GetStickerMessages Exception: {ex}");
            }
            return stickerMessages;
        }
    }
}