﻿using System.Text;
using BL.Service.Interface;
using BL.Service.Line;
using BL.Service.MapQuest;
using Core.Domain.DTO;
using Core.Domain.Enums;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using isRock.LineBot;

namespace BL.Service
{
    public class LineWebhookService : ILineWebhookService
    {
        private readonly ICambridgeDictionaryManager CambridgeDictionaryManager;
        private readonly IExchangeRateService ExchangeRateService;
        private readonly IMapQuestService MapQuestService;
        private readonly IMaskInstitutionService MaskInstitutionService;
        private readonly ITradingVolumeService TradingVolumeService;

        public LineWebhookService(
            ICambridgeDictionaryManager cambridgeDictionaryManager,
            IExchangeRateService exchangeRateService,
            IMaskInstitutionService maskInstitutionService,
            IMapQuestService mapQuestService,
            ITradingVolumeService tradingVolumeService)
        {
            CambridgeDictionaryManager = cambridgeDictionaryManager;
            ExchangeRateService = exchangeRateService;
            MaskInstitutionService = maskInstitutionService;
            MapQuestService = mapQuestService;
            TradingVolumeService = tradingVolumeService;
        }

        /// <summary>
        /// 依照事件取得Line回應訊息
        /// </summary>
        /// <param name="@event">事件</param>
        /// <returns>Line回應訊息</returns>
        public List<MessageBase> GetReplyMessages(Event @event)
        {
            List<MessageBase> replyMessages;
            string type = @event.type;
            if (type == "message")
            {
                Message message = @event.message;
                switch (message.type)
                {
                    case "text":
                        replyMessages = GetMessagesByText(message.text);
                        break;
                    case "location":
                        replyMessages = GetMaskInstitutions(message.address);
                        break;
                    case "sticker":
                        StickerMessage stickerMessage = LineMessageService.GetStickerMessage(message);
                        replyMessages = GetReplyMessagesBySticker(stickerMessage);
                        break;
                    default:
                        replyMessages = new List<MessageBase> { new TextMessage("目前未支援此資料格式: " + message.type) };
                        break;
                }
            }
            else if (type == "postback")
            {
                Postback postback = @event.postback;
                Params @params = postback.Params;
                if (postback.Params != null)
                {
                    replyMessages = GetMessagesByText(postback.data + " " + @params.datetime + @params.date + @params.time);
                }
                else
                {
                    replyMessages = GetMessagesByText(postback.data);
                }
            }
            else
            {
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
        public List<MessageBase> GetMessagesByText(string text)
        {
            string textStr;
            try
            {
                switch (text.Split(' ')[0])
                {
                    case "cd":
                        string vocabulary = text.Split(' ')[1];
                        return GetCambridgeDictionaryReplyMessages(vocabulary);
                    case "er":
                        return GetExchangeRateReplyMessages();
                    case "st":
                        string commandArg = text[3..];
                        return GetStickerReplyMessages(commandArg);
                    case "tv":
                        if (text == "tv")
                        {
                            return new List<MessageBase> { LineMessageService.GetCarouselTemplateMessage(QuerySortTypeEnum.Descending) };
                        }
                        if (text.Split(' ')[1].Length == 1)
                        {
                            int days = int.Parse(text.Split(' ')[1]);
                            if (days < 1 || days > 5)
                            {
                                textStr = "交易天數需為 1-5";
                            }
                            else
                            {
                                return TradingVolumeService.GetTradingVolumeStrOverDays(QuerySortTypeEnum.Descending, days);
                            }
                        }
                        else if (text.Split(' ')[1].Length == 10)
                        {
                            DateTime dateTime = DateTime.Parse(text.Split(' ')[1]);
                            return TradingVolumeService.GetTradingVolumeStr(dateTime, QuerySortTypeEnum.Descending);
                        }
                        else
                        {
                            textStr = $"請重新輸入! 參數錯誤({text.Split(' ')[1]})";
                        }
                        return new List<MessageBase> { LineMessageService.GetTextMessage(textStr) };
                    case "tvv":
                        if (text == "tvv")
                        {
                            return new List<MessageBase> { LineMessageService.GetCarouselTemplateMessage(QuerySortTypeEnum.Ascending) };
                        }
                        if (text.Split(' ')[1].Length == 1)
                        {
                            int days = int.Parse(text.Split(' ')[1]);
                            if (days < 1 || days > 5)
                            {
                                textStr = "交易天數需為 1-5";
                            }
                            else
                            {
                                return TradingVolumeService.GetTradingVolumeStrOverDays(QuerySortTypeEnum.Ascending, days);
                            }
                        }
                        else if (text.Split(' ')[1].Length == 10)
                        {
                            DateTime dateTime = DateTime.Parse(text.Split(' ')[1]);
                            return TradingVolumeService.GetTradingVolumeStr(dateTime, QuerySortTypeEnum.Ascending);
                        }
                        else
                        {
                            textStr = $"請重新輸入! 參數錯誤({text.Split(' ')[1]})";
                        }
                        return new List<MessageBase> { LineMessageService.GetTextMessage(textStr) };
                    default:
                        return new List<MessageBase> { LineMessageService.GetTextMessage(text) };
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"[GetMessagesByText] text: {text}, ex: {ex}";
                return new List<MessageBase> { LineMessageService.GetTextMessage(errorMsg) };
            }
        }

        /// <summary>
        /// 根據指定地址進行回應藥局資訊
        /// </summary>
        /// <param name="address">指定地址</param>
        /// <returns>LOG紀錄</returns>
        private List<MessageBase> GetMaskInstitutions(string address)
        {
            List<MaskInstitution> maskInstitutions = MaskInstitutionService.GetMaskInstitutions(address);

            Dictionary<string, MaskInstitution> maskInstitutionDict = maskInstitutions.ToDictionary(m => m.Address);

            List<string> orderedAddress = MapQuestService.GetAddressInOrderAsync(address, maskInstitutionDict.Keys.ToList()).Result;

            List<MaskInstitution> topFiveMaskInstitutions = new();
            for (int i = 0; i < 5 && i < orderedAddress.Count; i++)
            {
                topFiveMaskInstitutions.Add(maskInstitutionDict[orderedAddress[i]]);
            }

            if (topFiveMaskInstitutions.Count == 0)
            {
                return new List<MessageBase> { LineMessageService.GetTextMessage($"所在位置({address})沒有相關藥局") };
            }

            List<MessageBase> messages = new();
            foreach (MaskInstitution maskInstitution in topFiveMaskInstitutions)
            {
                var latLng = MapQuestService.GetLatLngAsync(maskInstitution.Address).Result;
                if (latLng.Lat == default || latLng.Lng == default)
                {
                    continue;
                }
                messages.Add(new LocationMessage(
                    maskInstitution.Name + "(成人: " + maskInstitution.NumberOfAdultMasks + "," +
                    "兒童: " + maskInstitution.NumberOfChildMasks + ")",
                    maskInstitution.Address,
                    latLng.Lat,
                    latLng.Lng
                ));
                if (messages.Count >= 5)
                {
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
        private List<MessageBase> GetCambridgeDictionaryReplyMessages(string vocabulary)
        {
            List<string> texts = GetCambridgeDictionaryTexts(vocabulary);

            List<MessageBase> messageBases = new();

            foreach (string text in texts)
            {
                MessageBase messageBase = LineMessageService.GetTextMessage(text);
                messageBases.Add(messageBase);
            }
            return messageBases;
        }

        /// <summary>
        /// 取得撈取劍橋辭典(CambridgeDictionary)網站的訊息列表
        /// </summary>
        /// <param name="vocabulary">單字</param>
        /// <returns>訊息列表</returns>
        private List<string> GetCambridgeDictionaryTexts(string vocabulary)
        {
            List<string> texts = new();
            List<Translation> translations =
                    CambridgeDictionaryManager.CrawlCambridgeDictionary(vocabulary).Take(5).ToList();

            if (translations.Count == 0)
            {
                texts.Add($"未能找到符合字詞: {vocabulary}");
                return texts;
            }

            // 設定發送的訊息
            foreach (Translation translation in translations)
            {
                string translationStr = translation.TranslationStr;
                if (translationStr.Length > 5000) // 防呆: 超過5000字數
                {
                    translationStr = string.Concat(translationStr.AsSpan(0, 4996), "...");
                }
                texts.Add(translationStr);
            }
            return texts;
        }

        /// <summary>
        /// 取得換匯(er)指令 的 回覆訊息列表
        /// </summary>
        /// <returns>訊息列表</returns>
        private List<MessageBase> GetExchangeRateReplyMessages()
        {
            ExchangeRateService.GetExchangeRate(
                out double buyingRate, out double sellingRate,
                out DateTime quotedDateTime);

            string textStr = ConvertToExchangeRateTextMessage(
                buyingRate, sellingRate, quotedDateTime
            );

            TextMessage textMessage = LineMessageService.GetTextMessage(textStr);

            return new List<MessageBase> { textMessage };
        }


        /// <summary>
        /// 將 換匯資訊 轉換成 字串
        /// </summary>
        /// <param name="bankBuyingRate">銀行買入匯率</param>
        /// <param name="bankSellingRate">銀行賣出匯率</param>
        /// <param name="quotedDateTime">報價時間</param>
        /// <returns>字串</returns>
        private static string ConvertToExchangeRateTextMessage(double bankBuyingRate, double bankSellingRate,
            DateTime quotedDateTime)
        {

            StringBuilder sb = new();
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
        /// <param name="commandArg">指令參數</param>
        /// <returns>訊息列表</returns>
        private static List<MessageBase> GetStickerReplyMessages(string commandArg)
        {
            if (commandArg.Split(' ').Length != 2 ||
                int.TryParse(commandArg.Split(' ')[0], out int packageId) == false ||
                int.TryParse(commandArg.Split(' ')[1], out int stickerId) == false)
            {

                TextMessage textMessage =
                    LineMessageService.GetTextMessage(
                        "此指令用來輸出最多五個的貼圖，\n" +
                        "用法：st {貼圖包Id} {貼圖Id}\n" +
                        "範例：st 1 1\n" +
                        "貼圖包/貼圖 如官方文件所定義：https://developers.line.biz/zh-hant/docs/messaging-api/sticker-list/#sticker-definitions");
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
        private static List<MessageBase> GetStickerReplyMessages(int packageId, int stickerId, int count)
        {
            if (count < 1 || count > 5)
                throw new ArgumentException("參數錯誤！個數必須介於 1 和 5。");

            List<MessageBase> messages = new();

            for (int i = stickerId; i < stickerId + count; ++i)
            {
                LineMessageService.TryGetStickerMessage(packageId, i, out MessageBase messageBase);
                messages.Add(messageBase);
            }

            return messages;
        }


        /// <summary>
        /// 取得貼圖訊息的回覆訊息列表
        /// </summary>
        /// <param name="stickerMessage">貼圖訊息</param>
        /// <returns>訊息列表</returns>
        private static List<MessageBase> GetReplyMessagesBySticker(StickerMessage stickerMessage)
        {
            GetIdsBySticker(stickerMessage, out int packageId, out int stickerId);
            string text = $"[StickerMessage] packageId: {packageId}, stickerId: {stickerId}";
            List<MessageBase> messages = new()
            {
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
        private static void GetIdsBySticker(StickerMessage stickerMessage, out int packageId, out int stickerId)
        {
            packageId = int.Parse(stickerMessage.packageId);
            stickerId = int.Parse(stickerMessage.stickerId);
        }
    }

    public class LineHttpPostException
    {
        public string Message { get; set; }
        public List<Detail> Details { get; set; }
    }

    public class Detail
    {
        public string Message { get; set; }
        public string Property { get; set; }
    }
}