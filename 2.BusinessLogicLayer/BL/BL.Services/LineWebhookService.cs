using BL.Services.Base;
using BL.Services.Google;
using BL.Services.Interfaces;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.DTO.Sinopac;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using DA.Managers.Interfaces.Sinopac;
using DA.Managers.MaskInstitution;
using DA.Managers.Sinopac;
using isRock.LineBot;
using Models.Google.API;
using Models.Line.Webhook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BL.Services {

    public class LineWebhookService : BaseService, ILineWebhookService {
        private readonly string _token;

        public LineWebhookService(string token) {
            CambridgeDictionaryManager = new CambridgeDictionaryManager();
            ExchangeRateManager = new ExchangeRateManager();
            _token = token;
        }

        private ICambridgeDictionaryManager CambridgeDictionaryManager { get; set; }
        private IExchangeRateManager ExchangeRateManager { get; set; }

        /// <summary>
        /// 回覆Line Server
        /// </summary>
        /// <param name="replyToken">回覆token</param>
        /// <param name="messages">訊息列表</param>
        /// <returns>API結果</returns>
        public string ResponseToLineServer(string replyToken, List<MessageBase> messages) {
            try {
                #region Post到Line
                Console.Write($"messages: {JsonConvert.SerializeObject(messages)}");
                Bot bot = new Bot(_token);
                string result = PostToLineServer(replyToken, messages);
                //string result = bot.ReplyMessage(replyToken, messages);
                #endregion Post到Line

                #region 若不成功則Post debug 訊息到Line
                Console.Write($"result: {result}");
                if (result != "{}") {
                    Console.Write(result);
                    string debugStr = $"messages:\n" +
                        $"{JsonConvert.SerializeObject(messages, Formatting.Indented)}\n";
                    if (result.StartsWith("伺服器無回應")) {
                        debugStr += "-> 伺服器無回應";
                    } else {
                        debugStr += "-> " + result;
                    }
                    var debugMessages = GetSingleMessage(debugStr);
                    bot.ReplyMessage(replyToken, debugMessages);
                }
                #endregion 若不成功則Post debug 訊息到Line

                return result;
            } catch (Exception ex) {
                Console.WriteLine(
                    $"LineWebhookService.Response 錯誤, replyToken: {replyToken},\n" +
                    $"messages: {JsonConvert.SerializeObject(messages, Formatting.Indented)}\n" +
                    $"ex: {ex}");
                return ex.ToString();
            }
        }

        /// <summary>
        /// 依照RequestModel取得Line回應訊息
        /// </summary>
        /// <param name="lineRequestModel"></param>
        /// <returns>Line回應訊息</returns>
        public List<MessageBase> GetReplyMessages(ReceivedMessage lineRequestModel) {
            Message message = lineRequestModel.events.FirstOrDefault().message;
            List<MessageBase> messages;
            switch ((string)message.type) {
                case "text":
                    messages = GetMessagesByText(message.text);
                    break;

                case "location":
                    messages = GetPharmacyInfoMessages(message.address);
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
            return messages;
        }

        /// <summary>
        /// 依照字串內容給於不同的 LINE 回應
        /// </summary>
        /// <param name="text">字串內容</param>
        /// <returns>回應結果</returns>
        private List<MessageBase> GetMessagesByText(string text) {
            List<MessageBase> messages = null;
            try {
                // Set up messages to send
                if (text.StartsWith("sp")) {
                    messages = GetSinopacMessages();
                } else if (text.StartsWith("sticker")) {
                    string packageIdStr = text.Split(' ')[1];
                    string stickerIdStr = text.Split(' ')[2];
                    messages = GetStickerMessages(packageIdStr, stickerIdStr);
                } else if (text.StartsWith(" ")) { // 倉頡用
                    messages = GetImageMessages(text.Substring(1));
                } else if (text.StartsWith("cd ")) {
                    string vocabulary = text.Split(' ')[1];
                    messages = GetCambridgeDictionaryMessages(vocabulary);
                } else {
                    messages = GetSingleMessage(text);
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex}");
            }
            return messages;
        }

        private List<MessageBase> GetSingleMessage(string text) {
            List<MessageBase> messages = new List<MessageBase>();
            try {
                messages.Add(new TextMessage(text));
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        private List<MessageBase> GetSinopacMessages() {
            List<ExchangeRate> exchangeRates = ExchangeRateManager.CrawlExchangeRate();

            Info info = exchangeRates[0].SubInfo[0];
            StringBuilder sb = new StringBuilder();
            sb.Append("美金報價\n");
            sb.Append("---------------------\n");
            sb.Append($"\t銀行買入：{info.DataValue2}\n");
            sb.Append($"\t銀行賣出：{info.DataValue3}");

            List<MessageBase> messages = new List<MessageBase>();
            messages.Add(new TextMessage(sb.ToString()));
            return messages;
        }

        /// <summary>
        /// 根據指定地址進行回應藥局資訊
        /// </summary>
        /// <param name="address">指定地址</param>
        /// <returns>LOG紀錄</returns>
        private List<MessageBase> GetPharmacyInfoMessages(string address) {
            List<MessageBase> messages = null;
            try {
                // 取得欲傳送的MaskDataList
                var topMaskDatas = MaskInstitutionManager.GetTopMaskDatasByComputingDistance(address, 5);

                // Set up messages to send
                messages = new List<MessageBase>();
                StringBuilder builder = new StringBuilder();

                if (topMaskDatas.Count == 0) {
                    builder.Append($"所在位置({address})沒有相關藥局");
                    messages.Add(new TextMessage(builder.ToString()));
                    return messages;
                }
                foreach (var maskData in topMaskDatas) {
                    Location location = MapService.GetGeocoding(maskData.Address).results[0].geometry.location;

                    if (!Double.TryParse(location.lat, out double lat)) {
                        Console.WriteLine($"Ex: Cannot parse {location.lat} to Int.");
                        lat = Double.MinValue;
                    }

                    if (!Double.TryParse(location.lng, out double lng)) {
                        Console.WriteLine($"Ex: Cannot parse {location.lng} to Int.");
                        lng = Double.MinValue;
                    }

                    messages.Add(new LocationMessage(
                        maskData.Name + "\n" +
                            "成人: " + maskData.AdultMasks + "\n" +
                            "兒童: " + maskData.ChildMasks,
                        maskData.Address,
                        lat, lng
                    ));
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex}");
            }
            return messages;
        }

        private List<MessageBase> GetStickerMessages(string packageId = "0", string stickerId = "0") {
            List<MessageBase> stickerMessages = null;
            try {
                // Set up messages to send
                stickerMessages = new List<MessageBase> {
                    new StickerMessage(1, 8),
                    new StickerMessage(1, 9),
                    new StickerMessage(1, 10),
                    new StickerMessage(1, 11),
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
        private List<MessageBase> GetCambridgeDictionaryMessages(string vocabulary) {
            List<MessageBase> messages = new List<MessageBase>();
            try {
                List<Translation> translations = CambridgeDictionaryManager.CrawlCambridgeDictionary(vocabulary);
                // 防呆: 超過5種詞性
                if (translations.Count > 5) {
                    translations = translations.Take(5).ToList();
                }

                // 設定發送的訊息
                foreach (Translation translation in translations) {
                    string translationStr = translation.TranslationStr;
                    // 防呆: 超過5000字數
                    if (translationStr.Length > 3000) {
                        translationStr = translationStr.Substring(0, 2996) + " ...";
                    }
                    messages.Add(new TextMessage(translationStr));
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        private List<MessageBase> GetImageMessages(string texts) {
            List<MessageBase> messages = new List<MessageBase>();
            try {
                Encoding big5 = Encoding.GetEncoding("big5");
                var CJDomain = "http://input.foruto.com/cjdict/Images/CJZD_JPG/";

                StringBuilder sb = new StringBuilder();
                foreach (var text in texts) {
                    if (text == ' ') {
                        continue;
                    }
                    // convert string to bytes
                    byte[] big5Bytes = big5.GetBytes(text.ToString());
                    var big5Str = BitConverter.ToString(big5Bytes).Replace("-", string.Empty);
                    sb.AppendLine(text + ": " + CJDomain + big5Str + ".JPG");
                    sb.AppendLine("--");
                }
                // Set up messages to send
                messages.Add(new TextMessage(sb.ToString()));
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        private List<dynamic> ReplyConfirmMessages() {
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

        #region 處理Line的requestBody

        /// <summary>
        /// 將requestBody轉換成Line的RequestModel
        /// </summary>
        /// <param name="requestBody">Line的將requestBody</param>
        /// <returns>Line的RequestModel</returns>
        //public ReceivedMessage GetLineRequestModel(dynamic requestBody) {
        //    ReceivedMessage lineRequestBody = JsonConvert.
        //        DeserializeObject<ReceivedMessage>(requestBody.ToString());
        //    foreach (isRock.LineBot.Event @event in lineRequestBody.events) {
        //        switch (@event.message.type) {
        //            case "text":
        //                @event.message = JsonConvert.DeserializeObject<TextMessage>(@event.message.ToString());
        //                break;

        //            case "location":
        //                @event.message = JsonConvert.DeserializeObject<LocationMessage>(@event.message.ToString());
        //                break;

        //            case "sticker":
        //                @event.message = JsonConvert.DeserializeObject<StickerMessage>(@event.message.ToString());
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //    return lineRequestBody;
        //}

        #endregion 處理Line的requestBody

        #region 處理Line的responseBody

        /// <summary>
        ///
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public string PostToLineServer(string replyToken, List<MessageBase> messages) {
            string result = "";
            try {
                string requestUriString = "https://api.line.me/v2/bot/message/reply";
                string channelAccessToken =
                    @"tkOO80fthaESrdEWkHn5+gsypQLHd1N3DZcNsWaJku3GeO/
                    HsFMyCSyU95KnA6p2bTLPFJS0y4joCknQyppqlwaDK34rrQgS
                    W39EcS0j5WNEZGIlkup0nJ+xlBf+mcw89H1xKAc5Ubd0xA9/Z
                    9RSIwdB04t89/1O/w1cDnyilFU=";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + channelAccessToken);

                // Write data to requestStream
                UTF8Encoding encoding = new UTF8Encoding();
                ReplyMessageRequestBody replyMessageRequestBody =
                    new ReplyMessageRequestBody(replyToken, messages);
                string requestBodyStr = JsonConvert.SerializeObject(replyMessageRequestBody, Formatting.Indented);
                byte[] data = encoding.GetBytes(requestBodyStr);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                // Add 紀錄發至LineServer的requestBody
                Console.WriteLine($"========== TO LINE SERVER: {requestUriString} ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBodyStr}");
                Console.WriteLine($"====================");

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result = streamReader.ReadToEnd();
            } catch (WebException webEx) {
                result += "伺服器無回應, " + webEx.ToString();
                Console.WriteLine($"伺服器無回應, WebException: {webEx}");
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex}");
            }
            return result;
        }

        #endregion 處理Line的responseBody
    }

    /// <summary>
    /// LINE 的 Reply Message 的 Request body
    /// </summary>
    public class ReplyMessageRequestBody {

        public ReplyMessageRequestBody(string replyToken, List<MessageBase> messages) {
            this.replyToken = replyToken;
            this.messages = messages;
        }

        /// <summary>
        /// 是否接收到通知
        /// </summary>
        public bool notificationDisabled { get; set; }

        /// <summary>
        /// webhook接收到的回應權杖
        /// </summary>
        public string replyToken { get; set; }

        /// <summary>
        /// 回覆的訊息列表，最多五則
        /// </summary>
        public List<MessageBase> messages { get; set; }
    }
}