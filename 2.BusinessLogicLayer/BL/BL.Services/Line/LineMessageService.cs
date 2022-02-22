using System;
using System.Collections.Generic;
using System.Text;
using BL.Services.Holiday;
using Core.Domain.Enums;
using isRock.LineBot;
using Serilog;

namespace BL.Services.Line {

    /// <summary>
    /// 產生需使用的Line Message
    /// </summary>
    public class LineMessageService {

        /// <summary>
        /// 將文字轉成 Line文字訊息
        /// </summary>
        /// <param name="text">文字</param>
        /// <returns>Line文字訊息</returns>
        public TextMessage GetTextMessage(string text) {
            try {
                text = text.Replace('\'', '’').Trim();
                return new TextMessage(text);
            } catch (Exception ex) {
                string errorMsg = $"[GetTextMessage] text: {text}, ex: {ex}";
                Log.Error(errorMsg);
                errorMsg = errorMsg.Replace('\'', '’').Trim();
                return new TextMessage(errorMsg);
            }
        }

        /// <summary>
        /// 轉換成 Line滑動訊息 By 搜尋排序類型
        /// </summary>
        /// <param name="querySortType">搜尋排序類型</param>
        /// <returns>Line滑動訊息</returns>
        /// <exception cref="ArgumentException">搜尋排序類型錯誤</exception>
        /// <remarks>所有Column必須數量相同</remarks>
        public MessageBase GetCarouselTemplateMessage(QuerySortTypeEnum querySortType) {
            string chineseWord, command;
            switch (querySortType) {
                case QuerySortTypeEnum.Ascending:
                    chineseWord = "賣超";
                    command = "tvv";
                    break;
                case QuerySortTypeEnum.Descending:
                    chineseWord = "買超";
                    command = "tv";
                    break;
                default:
                    throw new ArgumentException($"[GetCarouselTemplateMessage] 排序類型錯誤! (querySortType: {querySortType})");
            }
            List<DateTime> dates = HolidayHelper.GetTheMostRecentBusinessDay(2);
            List<Column> columns = new List<Column> {
                new Column() {
                    thumbnailImageUrl = new Uri("https://i.imgur.com/n82BOcq.png"),
                    title = $"計算外資及陸資，投信綜合{chineseWord}股數",
                    text = "請選擇計算區間:",
                    actions = new List<TemplateActionBase> {
                        new PostbackAction {
                            label = "一天內👉",
                            data = $"{command} 1",
                            displayText = $"我要查詢一天內綜合{chineseWord}股數🙏"
                        },
                        new PostbackAction {
                            label = "三天內👉",
                            data = $"{command} 3",
                            displayText = $"我要查詢三天內綜合{chineseWord}股數🙏"
                        },
                        new PostbackAction {
                            label = "五天內👉",
                            data = $"{command} 5",
                            displayText = $"我要查詢五天內綜合{chineseWord}股數🙏"
                        }
                    }
                },
                new Column() {
                    thumbnailImageUrl = new Uri("https://i.imgur.com/n82BOcq.png"),
                    title = $"計算外資及陸資，投信綜合{chineseWord}股數",
                    text = "請選擇日期:",
                    actions = new List<TemplateActionBase> {
                        new PostbackAction {
                            label = $"{dates[0]:yyyy/MM/dd}👉",
                            data = "tv " + dates[0].ToString("yyyy-MM-dd"),
                            displayText = $"我要查詢{dates[0]:yyyy/MM/dd}的綜合{chineseWord}股數🙏"
                        },
                        new PostbackAction {
                            label = $"{dates[1]:yyyy/MM/dd}👉",
                            data = "tv " + dates[1].ToString("yyyy-MM-dd"),
                            displayText = $"我要查詢{dates[1]:yyyy/MM/dd}的綜合{chineseWord}股數🙏"
                        },
                        new DateTimePickerAction {
                            label = "選擇日期👉",
                            data = "tv",
                            mode = "date",
                            initial = DateTime.UtcNow.AddHours(8).Date.ToString("yyyy-MM-dd"),
                            max = new DateTime(2025, 12, 31).ToString("yyyy-MM-dd"),
                            min = new DateTime(2011, 1, 1).ToString("yyyy-MM-dd")
                        }
                    }
                }
            };
            return new TemplateMessage(new CarouselTemplate() { columns = columns });
        }

        public MessageBase GetTextMessageWithQuickReply() {
            var quickReply = new QuickReply();
            var quickReplyMessageAction = new QuickReplyMessageAction("qr", "QuickReplyButton") {
                imageUrl = new Uri("https://imgur.com/ZQVKq9T.png"),
            };
            quickReply.items = new List<QuickReplyItemBase>{
                quickReplyMessageAction,
                new QuickReplyPostbackAction("0901", "tv 20200901", "九月一號", ""),
                new QuickReplyDatetimePickerAction("Select date", "storeId=12345", DatetimePickerModes.date),
                new QuickReplyCameraAction("Open Camera"),
                new QuickReplyCamerarollAction("Open Camera roll"),
                new QuickReplyLocationAction("Location1")
            };
            return new TextMessage("Please Select One.") { quickReply = quickReply };
        }

        public StickerMessage GetStickerMessage(Message stickerMessage) {
            int packageId = stickerMessage.packageId;
            int stickerId = stickerMessage.stickerId;
            return new StickerMessage(packageId, stickerId);
        }




        /// <summary>
        /// 嘗試取得 Line貼圖訊息
        /// </summary>
        /// <param name="packageId">貼圖包Id</param>
        /// <param name="stickerId">貼圖Id</param>
        /// <param name="stickerMessage">Line 貼圖訊息</param>
        /// <returns>Id是否有效</returns>
        public bool TryGetStickerMessage(int packageId, int stickerId, out MessageBase messageBase) {
            if (!IsValidPackageAndStickerId(packageId, stickerId)) {
                messageBase = new TextMessage($"此貼圖目前不支援輸出, packageId: {packageId}, stickerId: {stickerId}");
                return false;
            } else {
                messageBase = new StickerMessage(packageId, stickerId);
                return true;
            }
        }

        /// <summary>
        /// 判斷貼圖是否有效
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="stickerId"></param>
        /// <returns>是否有效</returns>
        /// <remarks>
        /// https://developers.line.biz/zh-hant/docs/messaging-api/sticker-list/#sticker-definitions <br/>
        /// 範圍包含： <br/>
        /// Moon James: {packageId: 1, stickerIds: 1-17, 21, 100-139, 401-430} <br/>
        /// Brown Cony: {packageId: 2, stickerIds: 18-20, 22-47, 140-179, 501-527} <br/>
        /// Cherry coco: {packageId: 3, stickerIds: 180-259}<br/>
        /// Daily Life: {packageId: 4, stickerIds: 260-307, 601-632}<br/>
        /// Brown, Cony &amp; Sally: {packageId: 11537, stickerIds: 52002734-52002773} <br/>
        /// CHOCO &amp; Friends: {packageId: 11538, stickerIds: 51626494-51626533} <br/>
        /// UNIVERSTAR BT21: {packageId: 11539, stickerIds: 52114110-52114149} <br/>
        /// </remarks>
        private bool IsValidPackageAndStickerId(int packageId, int stickerId) {
            switch (packageId) {
                case 1:
                    if (1 <= stickerId && stickerId <= 17) return true;
                    else if (stickerId == 21) return true;
                    else if (100 <= stickerId && stickerId <= 139) return true;
                    else if (401 <= stickerId && stickerId <= 430) return true;
                    else return false;
                case 2:
                    if (18 <= stickerId && stickerId <= 20) return true;
                    else if (22 <= stickerId && stickerId <= 47) return true;
                    else if (140 <= stickerId && stickerId <= 179) return true;
                    else if (501 <= stickerId && stickerId <= 527) return true;
                    else return false;
                case 3:
                    if (180 <= stickerId && stickerId <= 259) return true;
                    else return false;
                case 4:
                    if (260 <= stickerId && stickerId <= 307) return true;
                    else if (601 <= stickerId && stickerId <= 632) return true;
                    else return false;
                case 11537:
                    if (52002734 <= stickerId && stickerId <= 52002773) return true;
                    else return false;
                case 11538:
                    if (51626494 <= stickerId && stickerId <= 51626533) return true;
                    else return false;
                case 11539:
                    if (52114110 <= stickerId && stickerId <= 52114149) return true;
                    else return false;
                default:
                    return false;
            }
        }
    }
}