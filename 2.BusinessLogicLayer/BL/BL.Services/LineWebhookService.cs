using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BL.Services.Base;
using BL.Services.Interfaces;
using BL.Services.Line;
using BL.Services.TWSE_Stock;
using Core.Domain.DTO;
using Core.Domain.DTO.Map;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.DTO.Sinopac;
using Core.Domain.Enums;
using Core.Domain.Utilities;
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
        private readonly TradingVolumeService _tradingVolumeService;

        public LineWebhookService(
            IExchangeRateService exchangeRateService,
            IMapHereService mapHereService,
            IMaskInstitutionService maskInstitutionService) {
            _cambridgeDictionaryManager = new CambridgeDictionaryManager();
            _exchangeRateService = exchangeRateService;
            _lineMessageService = new LineMessageService();
            _maskInstitutionService = maskInstitutionService;
            _tradingVolumeService = new TradingVolumeService();
            _mapHereService = mapHereService;
        }

        /// <summary>
        /// 依照RequestModel 判讀訊息並傳回Line回應訊息
        /// </summary>
        /// <param name="receivedMessage">從line接收到的訊息字串</param>
        /// <returns>Line回應訊息</returns>
        public List<MessageBase> GetReplyMessages(ReceivedMessage receivedMessage) {
            List<MessageBase> messages;
            string type = receivedMessage.events.FirstOrDefault().type;
            if (type == "message") {
                Message message = receivedMessage.events.FirstOrDefault().message;
                switch (message.type) {
                    case "text":
                        messages = GetMessagesByText(message.text);
                        break;
                    case "location":
                        messages = GetMaskInstitutions(message.address);
                        break;
                    case "sticker":
                        StickerMessage stickerMessage = _lineMessageService.GetStickerMessage(message);
                        messages = GetMessageBySticker(stickerMessage);
                        break;
                    default:
                        messages = new List<MessageBase> { new TextMessage("目前未支援此資料格式: " + message.type) };
                        break;
                }
            } else if (type == "postback") {
                Postback postback = receivedMessage.events.FirstOrDefault().postback;
                Params @params = postback.Params;
                if (postback.Params != null) {
                    messages = GetMessagesByText(postback.data + " " + @params.datetime + @params.date + @params.time);
                } else {
                    messages = GetMessagesByText(postback.data);
                }
            } else {
                string errorMsg = $"[GetReplyMessages] 判讀訊息並傳回Line回應訊息 錯誤";
                throw new ArgumentException(errorMsg);
            }
            return messages;
        }

        private List<MessageBase> GetMessageBySticker(StickerMessage stickerMessage) {
            int packageId = int.Parse(stickerMessage.packageId);
            int stickerId = int.Parse(stickerMessage.stickerId);
            string text = $"[StickerMessage] packageId: {packageId}, stickerId: {stickerId}";
            TextMessage textMessage = new TextMessage(text);
            List<MessageBase> messages = new List<MessageBase>{
                textMessage
            };
            messages.AddRange(_lineMessageService.GetStickerMessages(packageId, stickerId, 4));
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
                switch (text.Split(' ')[0]) {
                    case "":
                        textStr = GetCangjieImageMessages(text.Substring(1));
                        return new List<MessageBase> { _lineMessageService.GetTextMessage(textStr) };
                    case "sp":
                        return _exchangeRateService.GetExchangeRateMessage();
                    case "st":
                        return GetStickerMessages(text);
                    case "cd":
                        string vocabulary = text.Split(' ')[1];
                        return GetCambridgeDictionaryMessages(vocabulary);
                    case "cdd":
                        vocabulary = text.Split(' ')[1];
                        int textLenth = int.Parse(text.Split(' ')[2]);
                        return GetCambridgeDictionaryMessages(vocabulary, textLenth);
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

        private List<MessageBase> GetStickerMessages(string text) {
            int packageId = int.Parse(text.Split(' ')[1]);
            int stickerId = int.Parse(text.Split(' ')[2]);
            if (text.Split(' ').Count() == 3) {
                return _lineMessageService.GetStickerMessages(packageId, stickerId);
            }

            int count = int.Parse(text.Split(' ')[3]);
            return _lineMessageService.GetStickerMessages(packageId, stickerId, count);
        }

        /// <summary>
        /// 根據指定地址進行回應藥局資訊
        /// </summary>
        /// <param name="address">指定地址</param>
        /// <returns>LOG紀錄</returns>
        private List<MessageBase> GetMaskInstitutions(string address) {
            List<MaskInstitution> topFiveMaskInstitutions = _maskInstitutionService.GetMaskInstitutionsByComputingDistance(address, 5);

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
                    // 防呆: 超過5000字數
                    if (textLength == -1) {
                        if (translationStr.Length > 5000) {
                            translationStr = translationStr.Substring(0, 4996) + "...";
                        }
                    } else if (translationStr.Length > textLength) {
                        translationStr = translationStr.Substring(0, textLength) + "...";
                    }
                    messages.Add(_lineMessageService.GetTextMessage(translationStr));
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
                    string big5Str = BitConverter.ToString(big5Bytes).Replace("-", string.Empty);
                    sb.AppendLine(text + ": " + CJDomain + big5Str + ".JPG");
                }
                return sb.ToString();
            } catch (Exception ex) {
                string errorMsg = $"發生錯誤, 字詞：{texts}, ex: {ex}";
                Console.WriteLine(errorMsg);
                return errorMsg;
            }
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