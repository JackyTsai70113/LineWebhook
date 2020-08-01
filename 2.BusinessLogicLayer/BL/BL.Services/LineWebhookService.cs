using BL.Interfaces;
using BL.Services.Base;
using Core.Domain.DTO.RequestDTO;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.DTO.ResponseDTO.Line;
using Core.Domain.DTO.ResponseDTO.Line.Messages;
using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates;
using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects;
using Core.Domain.Utilities;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utility.Google.MapAPIs;
using Utility.Line;
using Utility.MaskDatas;
using Utility.StringUtil;

namespace BL.Services {

    public class LineWebhookService : BaseService, ILineWebhookService {

        public LineWebhookService() {
            CambridgeDictionaryManager = new CambridgeDictionaryManager();
        }

        private RequestModelFromLineServer _LineRequestModel { get; set; }
        private ICambridgeDictionaryManager CambridgeDictionaryManager { get; set; }

        /// <summary>
        /// 判讀LineServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">LineServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response(dynamic requestBody) {
            string result = "";
            try {

                #region 處理RequestModel

                //處理requestModel
                RequestModelFromLineServer lineRequestModel =
                    LineRequestHandler.GetLineRequestModel(requestBody);

                Console.WriteLine($"========== From LINE SERVER ==========");
                Console.WriteLine($"requestModel:");
                Console.WriteLine($"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                Console.WriteLine($"====================");

                _LineRequestModel = lineRequestModel;

                #endregion 處理RequestModel

                // 判斷訊息型態，決定
                dynamic message = lineRequestModel.Events[0].message;
                switch ((string)message.type) {
                    case "text":
                        result = ReplyTextMessages(message.text);
                        break;

                    case "location":
                        string address = message.address;
                        result = ReplyPharmacyInfo(address);
                        break;

                    case "sticker":
                        result = ReplyStickerMessages();
                        break;

                    default:
                        Console.WriteLine($"無相符的 message.type: {(string)message.type}, " +
                            $"requestModelFromLineServer: " +
                            $"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                        result += ReplySameContentMessages("未支援此資料格式: " + (string)message.type);
                        break;
                }

                List<Message> messages = null;
                switch ((string)message.type) {
                    default:
                        Console.WriteLine($"無相符的 message.type: {(string)message.type}, " +
                            $"requestModelFromLineServer: " +
                            $"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                        messages = ReplySameContentMessagesTodo("未支援此資料格式: " + (string)message.type);
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
        /// 依照字串內容給於不同的 LINE 回應
        /// </summary>
        /// <param name="text">字串內容</param>
        /// <returns>回應結果</returns>
        private string ReplyTextMessages(string text) {
            string result = "";
            try {
                // Set up messages to send
                if (text.StartsWith("test")) {
                    result = ReplyTestMessages(text.Substring(4));
                } else if (text.StartsWith("sticker")) {
                    string packageIdStr = text.Split(' ')[1];
                    string stickerIdStr = text.Split(' ')[2];
                    result = ReplyStickerMessages(packageIdStr, stickerIdStr);
                } else if (text.StartsWith("cd ")) {
                    string vocabulary = text.Split(' ')[1];
                    result = ReplyCambridgeDictionaryMessages(vocabulary);
                } else if (text.StartsWith("sp")) {
                    text[2].ToString().TryParse(out int times);
                    string skey = text.Substring(63, 32);
                    result = ReplyShopeeMessages(times, skey);
                } else {
                    result = ReplySameContentMessages(text);
                }
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex}");
            }
            return result;
        }

        /// <summary>
        /// 回應 text 內容
        /// </summary>
        /// <returns>LOG紀錄</returns>
        private string ReplyTestMessages(string text) {
            string result = "";
            try {
                //var TextMessage = new
                //List<Message> messages2 = new List<Message> {
                //    new TextMessage {
                //        type = "text",
                //        text = text
                //    }
                //};
                //var replyMessageRequestBody = new ReplyMessageRequestBody {
                //    replyToken = "myToken",
                //    messages = messages2
                //};
                //var replyMessageRequestBody_SerializeObject = JsonConvert.SerializeObject(replyMessageRequestBody);
                ReplyMessageRequestBody replyMessageRequestBody =
                    JsonConvert.DeserializeObject<ReplyMessageRequestBody>(text);
                Message message = replyMessageRequestBody.messages.First();
                // Set up messages to send
                List<Message> messages = new List<Message> {
                    message
                };

                result = LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = _LineRequestModel.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// 回應 text 內容
        /// </summary>
        /// <param name="text">指定字串</param>
        /// <returns>LOG紀錄</returns>
        private string ReplySameContentMessages(string text) {
            string result = "";
            try {
                // Set up messages to send
                List<Message> messages = new List<Message> {
                    new TextMessage {
                        type = "text",
                        text = text
                    }
                };

                result = LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = _LineRequestModel.Events[0].replyToken,
                    messages = messages
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private List<Message> ReplySameContentMessagesTodo(string text) {
            List<Message> messages = null;
            try {
                // Set up messages to send
                messages = new List<Message> {
                    new TextMessage {
                        type = "text",
                        text = text
                    }
                };
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
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
                var messages = new List<Message>();
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

                result = LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = _LineRequestModel.Events[0].replyToken,
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
                List<Message> messages = new List<Message> {
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

                result = LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = _LineRequestModel.Events[0].replyToken,
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
                List<Translation> translations = CambridgeDictionaryManager.CrawlCambridgeDictionary(vocabulary);
                // 防呆: 超過5種詞性
                if (translations.Count > 5) {
                    translations = translations.Take(5).ToList();
                }

                // 設定發送的訊息
                string translationText = string.Join("\n", translations.Select(x => x.TranslationStr));
                List<Message> messages = new List<Message>();
                foreach (Translation translation in translations) {
                    string translationStr = translation.TranslationStr;
                    // 防呆: 超過5000字數
                    if (translationStr.Length > 5000) {
                        translationStr = translationStr.Substring(0, 4996) + " ...";
                    }
                    messages.Add(new TextMessage() {
                        text = translationStr
                    });
                }

                result = LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = _LineRequestModel.Events[0].replyToken,
                    messages = messages
                });

                // Set up messages to send
                List<Message> messages2 = new List<Message> {
                    new TextMessage {
                        type = "text",
                        text = result
                    }
                };

                LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                    replyToken = _LineRequestModel.Events[0].replyToken,
                    messages = messages2
                });
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private string ReplyConfirmMessages(int times, string skey) {
            string result = "";
            try {
                List<dynamic> messages = new List<dynamic> {
                    new {
                        type = "template",
                        altText = "this is a confirm template",
                        template = new {
                            type = "confirm",
                            text = "Are you sure?",
                            actions = new List<dynamic> {
                                new {
                                    type = "message",
                                    label = "Yes",
                                    text = "yes"
                                },
                                new {
                                    type = "message",
                                    label = "No",
                                    text = "no"
                                }
                            }
                        }
                    }
                };

                //result = ResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                //    replyToken = LineRequestBody.Events[0].replyToken,
                //    messages = messages
                //});
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        private string ReplyShopeeMessages(int times, string skey) {
            string uri = "https://games.shopee.tw/farm/api/friend/anonymous/help";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.Headers.Add("Content-Type", "application/json");

            // Write data to requestStream
            ASCIIEncoding encoding = new ASCIIEncoding();
            var requestBody = new {
                shareKey = skey,
                channel = "lineChat"
            };
            byte[] data = encoding.GetBytes(
                System.Text.Json.JsonSerializer.Serialize(requestBody));
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            //requestStream.WriteTimeout = 20000;
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            // Add 紀錄發至LineServer的requestBody
            string requestBodyStr = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
            Console.WriteLine($"========== TO Shopee SERVER: {uri} ==========");
            Console.WriteLine($"requestBody:");
            Console.WriteLine($"{requestBodyStr}");
            Console.WriteLine($"====================");

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            string result = streamReader.ReadToEnd();

            List<Message> messages = new List<Message> {
                    new TextMessage {
                        type = "text",
                        text = result
                    }
                };

            result = LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody {
                replyToken = _LineRequestModel.Events[0].replyToken,
                messages = messages
            });

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