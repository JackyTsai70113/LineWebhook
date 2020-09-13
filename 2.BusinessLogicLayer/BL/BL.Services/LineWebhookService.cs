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
using DA.Managers.CambridgeDictionary;
using DA.Managers.Sinopac;
using BL.Services.Line;

namespace BL.Services {

    public class LineWebhookService : BaseService, ILineWebhookService {
        private readonly string _token;

        public LineWebhookService(string token) {
            _cambridgeDictionaryManager = new CambridgeDictionaryManager();
            _exchangeRateManager = new ExchangeRateManager();
            _TradingVolumeService = new TradingVolumeService();
            _lineMessageService = new LineMessageService();
            _lineNotifyBotService = new LineNotifyBotService();
            _token = token;
        }

        private ICambridgeDictionaryManager _cambridgeDictionaryManager { get; set; }
        private IExchangeRateManager _exchangeRateManager { get; set; }

        private GeocodingService _GeocodingService { get; set; }
        private LineMessageService _lineMessageService { get; set; }
        private LineNotifyBotService _lineNotifyBotService { get; set; }
        private TradingVolumeService _TradingVolumeService { get; set; }

        /// <summary>
        /// 回覆Line Server
        /// </summary>
        /// <param name="replyToken">回覆token</param>
        /// <param name="messages">訊息列表</param>
        /// <returns>API結果</returns>
        public string ResponseToLineServer(string replyToken, List<MessageBase> messages) {
            try {
                #region Post到Line
                //Log.Information($"[ResponseToLineServer] messages: {JsonConvert.SerializeObject(messages)}");
                Bot bot = new Bot(_token);
                string result = bot.ReplyMessage(replyToken, messages);
                #endregion Post到Line
                return result;
            } catch (Exception ex) {
                int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
                int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                Log.Error("AA" + ex.ToString().Substring(responseStartIndex, responseEndIndex - responseStartIndex) + "AA");
                string responseStr = ex.ToString().Substring(responseStartIndex, responseEndIndex - responseStartIndex).Trim();
                LineHttpPostExceptionResponse response = JsonConvert.DeserializeObject<LineHttpPostExceptionResponse>(responseStr);
                Log.Error(
                    $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
                    $"messages: {JsonConvert.SerializeObject(messages, Formatting.Indented)}\n" +
                    $"response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
                _lineNotifyBotService.PushMessage_Jacky($"message: {response.message}, " +
                    $"details: {JsonConvert.SerializeObject(response.details, Formatting.Indented)}");

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
                    StickerMessage stickerMessage = _lineMessageService.GetStickerMessage(message);
                    int packageId = int.Parse(stickerMessage.packageId);
                    int stickerId = int.Parse(stickerMessage.stickerId);
                    string text = $"[StickerMessage] packageId: {packageId}, stickerId: {stickerId}";
                    TextMessage textMessage = new TextMessage(text);
                    messages = new List<MessageBase>{
                        textMessage
                    };
                    messages.AddRange(_lineMessageService.GetStickerMessages(packageId, stickerId, 4));
                    break;
                default:
                    Console.WriteLine($"無相符的 message.type: {message.type}, " +
                        $"requestModelFromLineServer: " +
                        $"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                    messages = _lineMessageService.GetSingleMessage("未支援此資料格式: " + message.type);
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
            string textStr;
            try {
                // Set up messages to send
                switch (text.Split(' ')[0]) {
                    case "":
                        textStr = GetCangjieImageMessages(text.Substring(1));
                        return _lineMessageService.GetSingleMessage(textStr);
                    case "sp":
                        textStr = GetSinopacExchangeRateText();
                        return _lineMessageService.GetSingleMessage(textStr);
                    case "st":
                        int packageId = int.Parse(text.Split(' ')[1]);
                        int stickerId = int.Parse(text.Split(' ')[2]);
                        if (text.Split(' ').Count() < 4) {
                            return _lineMessageService.GetStickerMessages(packageId, stickerId);
                        }

                        int count = int.Parse(text.Split(' ')[3]);
                        return _lineMessageService.GetStickerMessages(packageId, stickerId, count);
                    case "cd":
                        string vocabulary = text.Split(' ')[1];
                        return GetCambridgeDictionaryMessages(vocabulary);
                    case "cdd":
                        vocabulary = text.Split(' ')[1];
                        int textLenth = int.Parse(text.Split(' ')[2]);
                        return GetCambridgeDictionaryMessages(vocabulary, textLenth);
                    case "tv":
                        if (text == "tv") {
                            textStr = _TradingVolumeService.GetDescTradingVolumeStr(DateTime.UtcNow.AddHours(8));
                            return _lineMessageService.GetSingleMessage(textStr);
                        }
                        if (text.Split(' ')[1].Count() == 1) {
                            string daysStr = text.Split(' ')[1];
                            int days = int.Parse(daysStr);

                            if (days < 1 || days > 5) {
                                textStr = "交易天數需為 1-5";
                                return _lineMessageService.GetSingleMessage(textStr);
                            }

                            textStr = _TradingVolumeService.GetDescTradingVolumeStrOverDays(days);
                            return _lineMessageService.GetSingleMessage(textStr);
                        } else if (text.Split(' ')[1].Count() == 8) {
                            string dateTimeStr = text.Split(' ')[1];
                            DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyyMMdd", CultureInfo.InvariantCulture);

                            textStr = _TradingVolumeService.GetDescTradingVolumeStr(dateTime);
                            return _lineMessageService.GetSingleMessage(textStr);
                        }
                        textStr = "請重新輸入!";
                        return _lineMessageService.GetSingleMessage(textStr);
                    case "tvv":
                        if (text == "tvv") {
                            textStr = _TradingVolumeService.GetAscTradingVolumeStr(DateTime.UtcNow.AddHours(8));
                            return _lineMessageService.GetSingleMessage(textStr);
                        }
                        if (text.Split(' ')[1].Count() == 1) {
                            string daysStr = text.Split(' ')[1];
                            int days = int.Parse(daysStr);

                            if (days < 1 || days > 5) {
                                textStr = "交易天數需為 1-5";
                                return _lineMessageService.GetSingleMessage(textStr);
                            }

                            textStr = _TradingVolumeService.GetAscTradingVolumeStrOverDays(days);
                            return _lineMessageService.GetSingleMessage(textStr);
                        } else if (text.Split(' ')[1].Count() == 8) {
                            string dateTimeStr = text.Split(' ')[1];
                            DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyyMMdd", CultureInfo.InvariantCulture);

                            textStr = _TradingVolumeService.GetAscTradingVolumeStr(dateTime);
                            return _lineMessageService.GetSingleMessage(textStr);
                        }
                        textStr = "請重新輸入!";
                        return _lineMessageService.GetSingleMessage(textStr);
                    default:
                        return _lineMessageService.GetSingleMessage(text);
                }
            } catch (Exception ex) {
                string errorMsg = $"[GetMessagesByText] text: {text}, ex: {ex}";
                Log.Error(errorMsg);
                return _lineMessageService.GetSingleMessage(errorMsg);
            }
        }

        private string GetSinopacExchangeRateText() {
            List<ExchangeRate> exchangeRates = _exchangeRateManager.CrawlExchangeRate();
            Info info = exchangeRates[0].SubInfo[0];
            string titleInfo = exchangeRates[0].TitleInfo;
            titleInfo = StringUtility.StripHtmlTag(titleInfo);
            titleInfo = titleInfo.Substring(0, titleInfo.IndexOf('本'));
            StringBuilder sb = new StringBuilder();
            sb.Append("美金報價\n");
            sb.Append("---------------------\n");
            sb.Append($"({titleInfo})\n");
            sb.Append($"銀行買入：{info.DataValue2}\n");
            sb.Append($"銀行賣出：{info.DataValue3}");

            return sb.ToString();
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

        /// <summary>
        /// 取得撈取劍橋辭典(CambridgeDictionary)網站的訊息列表
        /// </summary>
        /// <param name="vocabulary">單字</param>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetCambridgeDictionaryMessages(string vocabulary, int textLength = -1) {
            List<MessageBase> messages = new List<MessageBase>();
            try {
                List<Translation> translations = _cambridgeDictionaryManager.CrawlCambridgeDictionary(vocabulary);
                if (translations.Count == 0) {
                    messages.Add(new TextMessage($"未能找到符合字詞: {vocabulary}"));
                    return messages;
                }
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
                return messages;
            } catch (Exception ex) {
                Log.Error($"vocabulary: {vocabulary}, ex: {ex}");
                return messages;
            }
        }

        private string GetCangjieImageMessages(string texts) {
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
                return sb.ToString();
            } catch (Exception ex) {
                string errorMsg = $"發生錯誤, 字詞：{texts}, ex: {ex}";
                Console.WriteLine(errorMsg);
                return errorMsg;
            }
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

    public class LineHttpPostExceptionResponse {
        public string message { get; set; }
        public List<Detail> details { get; set; }
    }
    public class Detail {
        public string message { get; set; }
        public string property { get; set; }
    }
}