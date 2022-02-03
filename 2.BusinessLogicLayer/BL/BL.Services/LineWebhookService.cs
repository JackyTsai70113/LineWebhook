using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BL.Services.Base;
using BL.Services.Interfaces;
using BL.Services.Line;
using Core.Domain.DTO;
using Core.Domain.DTO.Map;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.Enums;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using isRock.LineBot;
using Serilog;

namespace BL.Services {
    public class LineWebhookService : BaseService, ILineWebhookService {
        private readonly ICambridgeDictionaryManager _cambridgeDictionaryManager;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IMapHereService _mapHereService;
        private readonly IMaskInstitutionService _maskInstitutionService;
        private readonly LineMessageService _lineMessageService;
        private readonly ITradingVolumeService _tradingVolumeService;

        public LineWebhookService(
            IExchangeRateService exchangeRateService,
            IMapHereService mapHereService, ITradingVolumeService tradingVolumeService) {

            _cambridgeDictionaryManager = new CambridgeDictionaryManager();
            _exchangeRateService = exchangeRateService;
            _lineMessageService = new LineMessageService();
            _maskInstitutionService = new MaskInstitutionService();
            _tradingVolumeService = tradingVolumeService;
            _mapHereService = mapHereService;
        }

        /// <summary>
        /// 依照事件取得Line回應訊息
        /// </summary>
        /// <param name="@event">事件</param>
        /// <returns>Line回應訊息</returns>
        public List<MessageBase> GetReplyMessages(Event @event) {
            List<MessageBase> replyMessages;
            string type = @event.type;
            if (type == "message") {
                Message message = @event.message;
                switch (message.type) {
                    case "text":
                        replyMessages = GetMessagesByText(message.text);
                        break;
                    case "location":
                        replyMessages = GetMaskInstitutions(message.address);
                        break;
                    case "sticker":
                        StickerMessage stickerMessage = _lineMessageService.GetStickerMessage(message);
                        replyMessages = GetReplyMessagesBySticker(stickerMessage);
                        break;
                    default:
                        replyMessages = new List<MessageBase> { new TextMessage("目前未支援此資料格式: " + message.type) };
                        break;
                }
            } else if (type == "postback") {
                Postback postback = @event.postback;
                Params @params = postback.Params;
                if (postback.Params != null) {
                    replyMessages = GetMessagesByText(postback.data + " " + @params.datetime + @params.date + @params.time);
                } else {
                    replyMessages = GetMessagesByText(postback.data);
                }
            } else {
                string errorMsg = $"[GetReplyMessages] 判讀訊息並傳回Line回應訊息 錯誤";
                throw new ArgumentException(errorMsg);
            }
            return replyMessages;
        }

        /// <summary>
        /// 依照字串內容給於不同的 LINE 回應
        /// </summary>
        /// <param name="text">字串內容</param>
        /// <returns>回應結果</returns>
        public List<MessageBase> GetMessagesByText(string text) {
            string textStr;
            try {
                switch (text.Split(' ')[0]) {
                    case "cd":
                        string vocabulary = text.Split(' ')[1];
                        return GetCambridgeDictionaryReplyMessages(vocabulary);
                    case "cj":
                        string words = text.Substring(3);
                        return GetCangjieReplyMessages(words);
                    case "er":
                        return GetExchangeRateReplyMessages();
                    case "st":
                        string[] commandArgs = text.Substring(3).Split(' ');
                        return GetStickerReplyMessages(commandArgs);
                    case "tv":
                        if (text == "tv") {
                            return new List<MessageBase> { _lineMessageService.GetCarouselTemplateMessage(QuerySortTypeEnum.Descending) };
                        }
                        if (text.Split(' ')[1].Count() == 1) {
                            int days = int.Parse(text.Split(' ')[1]);
                            if (days < 1 || days > 5) {
                                textStr = "交易天數需為 1-5";
                            } else {
                                return _tradingVolumeService.GetTradingVolumeStrOverDays(QuerySortTypeEnum.Descending, days);
                            }
                        } else if (text.Split(' ')[1].Count() == 10) {
                            DateTime dateTime = DateTime.Parse(text.Split(' ')[1]);
                            return _tradingVolumeService.GetTradingVolumeStr(dateTime, QuerySortTypeEnum.Descending);
                        } else {
                            textStr = $"請重新輸入! 參數錯誤({text.Split(' ')[1]})";
                        }
                        return new List<MessageBase> { _lineMessageService.GetTextMessage(textStr) };
                    case "tvv":
                        if (text == "tvv") {
                            return new List<MessageBase> { _lineMessageService.GetCarouselTemplateMessage(QuerySortTypeEnum.Ascending) };
                        }
                        if (text.Split(' ')[1].Count() == 1) {
                            int days = int.Parse(text.Split(' ')[1]);
                            if (days < 1 || days > 5) {
                                textStr = "交易天數需為 1-5";
                            } else {
                                return _tradingVolumeService.GetTradingVolumeStrOverDays(QuerySortTypeEnum.Ascending, days);
                            }
                        } else if (text.Split(' ')[1].Count() == 10) {
                            DateTime dateTime = DateTime.Parse(text.Split(' ')[1]);
                            return _tradingVolumeService.GetTradingVolumeStr(dateTime, QuerySortTypeEnum.Ascending);
                        } else {
                            textStr = $"請重新輸入! 參數錯誤({text.Split(' ')[1]})";
                        }
                        return new List<MessageBase> { _lineMessageService.GetTextMessage(textStr) };
                    default:
                        return new List<MessageBase> { _lineMessageService.GetTextMessage(text) };
                }
            } catch (Exception ex) {
                string errorMsg = $"[GetMessagesByText] text: {text}, ex: {ex}";
                Log.Error(errorMsg);
                return new List<MessageBase> { _lineMessageService.GetTextMessage(errorMsg) };
            }
        }

        /// <summary>
        /// 根據指定地址進行回應藥局資訊
        /// </summary>
        /// <param name="address">指定地址</param>
        /// <returns>LOG紀錄</returns>
        private List<MessageBase> GetMaskInstitutions(string address) {
            List<MaskInstitution> maskInstitutions = _maskInstitutionService.GetMaskInstitutions(address);

            Dictionary<string, MaskInstitution> maskInstitutionDict = maskInstitutions.ToDictionary(m => m.Address);

            List<string> orderedAddress = _mapHereService.GetAddressInOrder(address, maskInstitutionDict.Keys.ToList());

            List<MaskInstitution> topFiveMaskInstitutions = new List<MaskInstitution>();
            for (int i = 0; i < 5 && i < orderedAddress.Count(); i++) {
                topFiveMaskInstitutions.Add(maskInstitutionDict[orderedAddress[i]]);
            }

            if (topFiveMaskInstitutions.Count == 0) {
                return new List<MessageBase> { _lineMessageService.GetTextMessage($"所在位置({address})沒有相關藥局") };
            }

            List<MessageBase> messages = new List<MessageBase>();
            foreach (MaskInstitution maskInstitution in topFiveMaskInstitutions) {
                LatLng latLng = _mapHereService.GetLatLngFromAddress(maskInstitution.Address);
                if (latLng.lat == default || latLng.lng == default) {
                    continue;
                }
                messages.Add(new LocationMessage(
                    maskInstitution.Name + "\n" +
                    "成人: " + maskInstitution.numberOfAdultMasks + "\n" +
                    "兒童: " + maskInstitution.numberOfChildMasks,
                    maskInstitution.Address,
                    latLng.lat,
                    latLng.lng
                ));
                if (messages.Count >= 5) {
                    break;
                }
            }
            return messages;
        }

        /// <summary>
        /// 取得劍橋(cd)指令的 回覆訊息列表
        /// </summary>
        /// <param name="vocabulary">單字</param>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetCambridgeDictionaryReplyMessages(string vocabulary) {
            List<string> texts = GetCambridgeDictionaryTexts(vocabulary);

            List<MessageBase> messageBases = new List<MessageBase>();

            foreach (string text in texts) {
                MessageBase messageBase = _lineMessageService.GetTextMessage(text);
                messageBases.Add(messageBase);
            }
            return messageBases;
        }

        /// <summary>
        /// 取得撈取劍橋辭典(CambridgeDictionary)網站的訊息列表
        /// </summary>
        /// <param name="vocabulary">單字</param>
        /// <returns>訊息列表</returns>
        private List<string> GetCambridgeDictionaryTexts(string vocabulary) {
            List<string> texts = new List<string>();
            try {
                List<Translation> translations =
                    _cambridgeDictionaryManager.CrawlCambridgeDictionary(vocabulary)
                                               .Take(5).ToList();

                if (translations.Count == 0) {
                    texts.Add($"未能找到符合字詞: {vocabulary}");
                    return texts;
                }

                // 設定發送的訊息
                foreach (Translation translation in translations) {
                    string translationStr = translation.TranslationStr;
                    // 防呆: 超過5000字數
                    if (translationStr.Length > 5000) {
                        translationStr = translationStr.Substring(0, 4996) + "...";
                    }
                    texts.Add(translationStr);
                }
                return texts;
            } catch (Exception ex) {
                Log.Error($"取得撈取劍橋辭典網站的訊息列表 錯誤, vocabulary: {vocabulary}, ex: {ex}");
                return texts;
            }
        }

        /// <summary>
        /// 取得倉頡(cj)指令的 回覆訊息列表
        /// </summary>
        /// <param name="words">文字列表</param>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetCangjieReplyMessages(string words) {

            string replyTextStr = GetCangjieReplyText(words);

            TextMessage textMessage = _lineMessageService.GetTextMessage(replyTextStr);

            return new List<MessageBase> { textMessage };
        }

        /// <summary>
        /// 整理倉頡文字的分解方法，並回傳整理好的文字
        /// </summary>
        /// <param name="words">倉頡文字列表</param>
        /// <returns>文字</returns>
        private string GetCangjieReplyText(string words) {
            try {
                Encoding big5 = Encoding.GetEncoding("big5");
                string cjDomain = "http://input.foruto.com/cjdict/Images/CJZD_JPG/";

                StringBuilder sb = new StringBuilder();
                foreach (char word in words) {
                    if (word == ' ') {
                        continue;
                    }
                    // convert string to bytes
                    byte[] big5Bytes = big5.GetBytes(word.ToString());
                    string big5Str = BitConverter.ToString(big5Bytes).Replace("-", string.Empty);
                    sb.AppendLine(word + ": " + cjDomain + big5Str + ".JPG");
                }
                string cangjieReplyText = sb.ToString();
                return cangjieReplyText;
            } catch (Exception ex) {
                string errorMsg = $"GetCangjieReplyText 發生錯誤, 字詞：{words}, ex: {ex}";
                Log.Error(errorMsg);
                return errorMsg;
            }
        }

        /// <summary>
        /// 取得換匯(er)指令 的 回覆訊息列表
        /// </summary>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetExchangeRateReplyMessages() {
            _exchangeRateService.GetExchangeRate(
                out double buyingRate, out double sellingRate,
                out DateTime quotedDateTime);

            string textStr = ConvertToExchangeRateTextMessage(
                buyingRate, sellingRate, quotedDateTime
            );

            TextMessage textMessage = _lineMessageService.GetTextMessage(textStr);

            return new List<MessageBase> { textMessage };
        }


        /// <summary>
        /// 將 換匯資訊 轉換成 字串
        /// </summary>
        /// <param name="bankBuyingRate">銀行買入匯率</param>
        /// <param name="bankSellingRate">銀行賣出匯率</param>
        /// <param name="quotedDateTime">報價時間</param>
        /// <returns>字串</returns>
        private string ConvertToExchangeRateTextMessage(double bankBuyingRate, double bankSellingRate,
            DateTime quotedDateTime) {

            StringBuilder sb = new StringBuilder();
            sb.Append("美金報價\n");
            sb.Append("---------------------\n");
            sb.Append($"銀行買入：{bankBuyingRate: 0.0000}\n");
            sb.Append($"銀行賣出：{bankSellingRate: 0.0000}\n");
            sb.Append($"報價時間：{quotedDateTime: yyyy-MM-dd HH:mm:ss}");

            string result = sb.ToString();

            return result;
        }

        /// <summary>
        /// 取得貼圖(st)指令 的 回覆訊息列表
        /// </summary>
        /// <param name="commandArgs">指令參數矩陣</param>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetStickerReplyMessages(string[] commandArgs) {

            if (int.TryParse(commandArgs[0], out int packageId) == false ||
                int.TryParse(commandArgs[1], out int stickerId) == false) {

                Log.Information("GetStickerMessages 解析st指令錯誤，格式錯誤"
                    + $"，commandArgs: {commandArgs[0]}, {commandArgs[1]}");
                TextMessage textMessage = _lineMessageService.GetTextMessage("格式錯誤");
                return new List<MessageBase> { textMessage };
            }

            List<MessageBase> messageBases = GetStickerReplyMessages(packageId, stickerId, 5);

            return messageBases;
        }

        /// <summary>
        /// 取得貼圖的 回覆訊息列表
        /// </summary>
        /// <param name="packageId">貼圖包Id</param>
        /// <param name="stickerId">貼圖Id</param>
        /// <param name="count">Line 訊息數量</param>
        /// <returns>訊息列表</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// count 必須介於 1 和 5
        /// </exception>
        private List<MessageBase> GetStickerReplyMessages(int packageId, int stickerId, int count) {
            if (count == 1) {
                _lineMessageService.TryGetStickerMessage(packageId, stickerId, out MessageBase messageBase);
                return new List<MessageBase> { messageBase };
            }

            if (count < 1 || count > 5) {
                throw new ArgumentException("參數錯誤！個數必須介於 1 和 5。");
            }

            List<MessageBase> messages = new List<MessageBase>();

            for (int i = stickerId; i < stickerId + count; ++i) {
                _lineMessageService.TryGetStickerMessage(packageId, i, out MessageBase messageBase);
                messages.Add(messageBase);
            }

            return messages;
        }


        /// <summary>
        /// 取得貼圖訊息的回覆訊息列表
        /// </summary>
        /// <param name="stickerMessage">貼圖訊息</param>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetReplyMessagesBySticker(StickerMessage stickerMessage) {
            GetIdsBySticker(stickerMessage, out int packageId, out int stickerId);
            string text = $"[StickerMessage] packageId: {packageId}, stickerId: {stickerId}";
            List<MessageBase> messages = new List<MessageBase>{
                new TextMessage(text)
            };

            List<MessageBase> stickerMessages = GetStickerReplyMessages(packageId, stickerId, 4);
            messages.AddRange(stickerMessages);

            return messages;
        }

        /// <summary>
        /// 取得Line貼圖訊息的貼圖包Id, 貼圖Id
        /// </summary>
        /// <param name="stickerMessage">Line貼圖訊息</param>
        /// <param name="packageId">貼圖包Id</param>
        /// <param name="stickerId">貼圖Id</param>
        private void GetIdsBySticker(StickerMessage stickerMessage, out int packageId, out int stickerId) {
            packageId = int.Parse(stickerMessage.packageId);
            stickerId = int.Parse(stickerMessage.stickerId);
        }
    }

    public class LineHttpPostException {
        public string message { get; set; }
        public List<Detail> details { get; set; }
    }

    public class Detail {
        public string message { get; set; }
        public string property { get; set; }
    }
}