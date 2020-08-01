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

                #region 判斷RequestModel決定回傳給Line的訊息

                dynamic message = lineRequestModel.Events[0].message;
                List<Message> messages = null;
                switch ((string)message.type) {
                    case "text":
                        messages = GetMessagesByText(message.text);
                        break;

                    case "location":
                        string address = message.address;
                        messages = GetPharmacyInfoMessages(address);
                        break;

                    case "sticker":
                        messages = GetStickerMessages();
                        break;

                    default:
                        Console.WriteLine($"無相符的 message.type: {(string)message.type}, " +
                            $"requestModelFromLineServer: " +
                            $"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                        messages = GetSingleMessage("未支援此資料格式: " + (string)message.type);
                        break;
                }

                #endregion 判斷RequestModel決定回傳給Line的訊息

                #region Post到Line

                ReplyMessageRequestBody replyMessageRequestBody =
                    new ReplyMessageRequestBody(_LineRequestModel.Events[0].replyToken, messages);
                result = LineResponseHandler.PostToLineServer(replyMessageRequestBody);

                #endregion Post到Line

                #region 若不成功則Post debug 訊息到Line

                if (result != "{}") {
                    string debugStr = $"messages:\n" +
                        $"{JsonConvert.SerializeObject(messages, Formatting.Indented)}\n";
                    if (result.StartsWith("伺服器無法取得回應")) {
                        debugStr += "-> 伺服器無法取得回應";
                    } else {
                        debugStr += result;
                    }
                    var debugMessages = GetSingleMessage(debugStr);
                    LineResponseHandler.PostToLineServer(new ReplyMessageRequestBody(_LineRequestModel.Events[0].replyToken, debugMessages));
                }

                #endregion 若不成功則Post debug 訊息到Line

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
        private List<Message> GetMessagesByText(string text) {
            List<Message> messages = null;
            try {
                // Set up messages to send
                if (text.StartsWith("test")) {
                    messages = ReplyTestMessages(text.Substring(4));
                } else if (text.StartsWith("sticker")) {
                    string packageIdStr = text.Split(' ')[1];
                    string stickerIdStr = text.Split(' ')[2];
                    messages = GetStickerMessages(packageIdStr, stickerIdStr);
                } else if (text.StartsWith("cd ")) {
                    string vocabulary = text.Split(' ')[1];
                    messages = GetCDMessages(vocabulary);
                } else {
                    messages = GetSingleMessage(text);
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex}");
            }
            return messages;
        }

        /// <summary>
        /// 回應 text 內容
        /// </summary>
        /// <returns>LOG紀錄</returns>
        private List<Message> ReplyTestMessages(string text) {
            List<Message> messages = null;
            try {
                ReplyMessageRequestBody replyMessageRequestBody =
                    JsonConvert.DeserializeObject<ReplyMessageRequestBody>(text);
                Message message = replyMessageRequestBody.messages.First();
                // Set up messages to send
                messages = new List<Message> { message };
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        private List<Message> GetSingleMessage(string text) {
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
        private List<Message> GetPharmacyInfoMessages(string address) {
            List<Message> messages = null;
            try {
                // 取得欲傳送的MaskDataList
                var topMaskDatas = MaskDataHandler.GetTopMaskDatasByComputingDistance(address, 5);

                // Set up messages to send
                messages = new List<Message>();
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
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        private List<Message> GetStickerMessages(string packageId = "0", string stickerId = "0") {
            List<Message> stickerMessages = null;
            try {
                // Set up messages to send
                stickerMessages = new List<Message> {
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

        /// <summary>
        /// 取得撈取劍橋辭典(CambridgeDictionary)網站的訊息列表
        /// </summary>
        /// <param name="vocabulary">單字</param>
        /// <returns>訊息列表</returns>
        private List<Message> GetCDMessages(string vocabulary) {
            List<Message> messages = new List<Message>();
            try {
                List<Translation> translations = CambridgeDictionaryManager.CrawlCambridgeDictionary(vocabulary);
                // 防呆: 超過5種詞性
                if (translations.Count > 5) {
                    translations = translations.Take(5).ToList();
                }

                // 設定發送的訊息
                string translationText = string.Join("\n", translations.Select(x => x.TranslationStr));
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
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        private List<dynamic> ReplyConfirmMessages(int times, string skey) {
            List<dynamic> messages = null;
            try {
                messages = new List<dynamic> {
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
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }
    }
}