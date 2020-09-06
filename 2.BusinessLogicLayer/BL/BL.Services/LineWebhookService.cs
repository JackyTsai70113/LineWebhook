using BL.Services.Base;
using BL.Services.Interfaces;
using BL.Services.MapQuest;
using BL.Services.TWSE_Stock;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.DTO.Sinopac;
using DA.Managers.Interfaces;
using DA.Managers.Interfaces.Sinopac;
using DA.Managers.MaskInstitution;
using isRock.LineBot;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Core.Domain.Utilities;

namespace BL.Services {

    public class LineWebhookService : BaseService, ILineWebhookService {
        private readonly string _token;

        public LineWebhookService(string token) {
            _TradingVolumeService = new TradingVolumeService();
            _token = token;
        }

        private ICambridgeDictionaryManager CambridgeDictionaryManager { get; set; }
        private IExchangeRateManager ExchangeRateManager { get; set; }
        private TradingVolumeService _TradingVolumeService { get; set; }
        private GeocodingService _GeocodingService { get; set; }

        /// <summary>
        /// 回覆Line Server
        /// </summary>
        /// <param name="replyToken">回覆token</param>
        /// <param name="messages">訊息列表</param>
        /// <returns>API結果</returns>
        public string ResponseToLineServer(string replyToken, List<MessageBase> messages) {
            try {
                #region Post到Line
                Log.Information($"[ResponseToLineServer] messages: {JsonConvert.SerializeObject(messages)}");
                Bot bot = new Bot(_token);
                string result = bot.ReplyMessage(replyToken, messages);
                #endregion Post到Line

                #region 若不成功則Post debug 訊息到Line
                if (result != "{}") {
                    Log.Error(result);
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
                Log.Error(
                    $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
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
            switch (message.type) {
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
            string textStr;
            try {
                //messages = GetSingleMessage(text);
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
                } else if (text.StartsWith("cdd ")) {
                    string vocabulary = text.Split(' ')[1];
                    int textLenth = int.Parse(text.Split(' ')[2]);
                    messages = GetCambridgeDictionaryMessages(vocabulary, textLenth);
                } else if (text.StartsWith("tv")) {
                    if (text.Count() == 2) {
                        throw new ArgumentException("請重新輸入...tv1 或 tv2 ?");
                    }
                    bool isDesc;
                    switch (text[2]) {
                        case '1':
                            isDesc = true;
                            break;
                        case '2':
                            isDesc = false;
                            break;
                        default:
                            throw new ArgumentException("參數錯誤");
                    }
                    if (text.Split(' ').Count() == 1) {
                        if (isDesc) {
                            textStr = _TradingVolumeService.GetDescTradingVolumeStr(DateTime.UtcNow.AddHours(8));
                        } else {
                            textStr = _TradingVolumeService.GetAscTradingVolumeStr(DateTime.UtcNow.AddHours(8));
                        }
                        messages = GetSingleMessage(textStr);
                    } else if (text.Split(' ')[1].Count() == 1) {
                        string daysStr = text.Split(' ')[1];
                        int days = int.Parse(daysStr);
                        if (days < 1 || days > 5) {
                            textStr = "交易天數需為 1-5";
                            messages = GetSingleMessage(textStr);
                        } else {
                            if (isDesc) {
                                textStr = _TradingVolumeService.GetDescTradingVolumeStrOverDays(days);
                            } else {
                                textStr = _TradingVolumeService.GetAscTradingVolumeStrOverDays(days);
                            }
                            messages = GetSingleMessage(textStr);
                        }
                    } else if (text.Split(' ')[1].Count() == 8) {
                        string dateTimeStr = text.Split(' ')[1];
                        DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyyMMdd", CultureInfo.InvariantCulture);
                        if (isDesc) {
                            textStr = _TradingVolumeService.GetDescTradingVolumeStr(dateTime);
                        } else {
                            textStr = _TradingVolumeService.GetAscTradingVolumeStr(dateTime);
                        }
                        messages = GetSingleMessage(textStr);
                    } else {
                        throw new ArgumentException("第二個參數錯誤");
                    }
                } else {
                    messages = GetSingleMessage(text);
                }
            } catch (Exception ex) {
                string errorMsg = $"[GetMessagesByText] text: {text}, ex: {ex}";
                Log.Error(errorMsg);
                messages = GetSingleMessage(errorMsg);
            }
            return messages;
        }

        private List<MessageBase> GetSingleMessage(string text) {
            List<MessageBase> messages = new List<MessageBase>();
            try {
                messages.Add(new TextMessage(text.Trim()));
            } catch (Exception ex) {
                Console.WriteLine($"[GetSingleMessage] Exception: {ex.Message}");
            }
            return messages;
        }

        private List<MessageBase> GetSinopacMessages() {
            List<ExchangeRate> exchangeRates = ExchangeRateManager.CrawlExchangeRate();

            Info info = exchangeRates[0].SubInfo[0];
            StringBuilder sb = new StringBuilder();
            sb.Append("美金報價\n");
            sb.Append("---------------------\n");
            sb.Append($"銀行買入：{info.DataValue2}\n");
            sb.Append($"銀行賣出：{info.DataValue3}");

            List<MessageBase> messages = new List<MessageBase> {
                new TextMessage(sb.ToString())
            };
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
                var topMaskDatas = MaskInstitutionManager.GetTopMaskDatasBySecondDivision(address, 20);

                // Set up messages to send
                messages = new List<MessageBase>();
                StringBuilder builder = new StringBuilder();

                if (topMaskDatas.Count == 0) {
                    builder.Append($"所在位置({address})沒有相關藥局");
                    messages.Add(new TextMessage(builder.ToString()));
                    return messages;
                }
                foreach (var maskData in topMaskDatas) {
                    LatLng latLng = _GeocodingService.GetLatLngFromAddress(maskData.Address);
                    if (latLng.lat == default || latLng.lng == default) {
                        continue;
                    }
                    messages.Add(new LocationMessage(
                        maskData.Name + "\n" +
                        "成人: " + maskData.AdultMasks + "\n" +
                        "兒童: " + maskData.ChildMasks,
                        maskData.Address,
                        latLng.lat,
                        latLng.lng
                    ));
                    if (messages.Count >= 5) {
                        break;
                    }
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
        private List<MessageBase> GetCambridgeDictionaryMessages(string vocabulary, int textLength = -1) {
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
                    translationStr = translationStr.Replace('\'', '’').TrimEnd();
                    // 防呆: 超過5000字數
                    if (textLength == -1) {
                        if (translationStr.Length > 5000) {
                            translationStr = translationStr.Substring(0, 4996) + "...";
                        }
                    } else if (translationStr.Length > textLength) {
                        translationStr = translationStr.Substring(0, textLength) + "...";
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

        public List<dynamic> ReplyConfirmMessages() {
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + _token);

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