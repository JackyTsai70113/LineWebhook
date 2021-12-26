﻿using System;
using System.Collections.Generic;
using BL.Services.Holiday;
using Core.Domain.Enums;
using isRock.LineBot;
using Serilog;

namespace BL.Services.Line {

    /// <summary>
    /// 產生需使用的Line Message
    /// </summary>
    public class LineMessageService {

        public MessageBase GetTextMessage(string text) {
            try {
                text = text.Replace('\'', '’').Trim();
                return new TextMessage(text);
            } catch (Exception ex) {
                string errorMsg = $"[GetSingleMessage] text: {text} Exception: {ex}";
                Log.Error(errorMsg);
                errorMsg = errorMsg.Replace('\'', '’').Trim();
                return new TextMessage(errorMsg);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
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
        /// 取得貼圖訊息
        /// </summary>
        /// <remarks>
        /// https://developers.line.biz/media/messaging-api/sticker_list.pdf <br/>
        /// https://devdocs.line.me/files/sticker_list.pdf <br/>
        /// 範圍包含： <br/>
        /// Moon James: {packageId: 1, stickerIds: 1-17, 21, 100-139, 401-430} <br/>
        /// Brown Cony: {packageId: 2, stickerIds: 18-20, 22-47, 140-179, 501-527} <br/>
        /// Cherry coco: {packageId: 3, stickerIds: 180-259}<br/>
        /// Daily Life: {packageId: 4, stickerIds: 260-307, 601-632}<br/>
        /// Brown, Cony &amp; Sally: {packageId: 11537, stickerIds: 52002734-52002773} <br/>
        /// CHOCO &amp; Friends: {packageId: 11538, stickerIds: 51626494-51626533} <br/>
        /// UNIVERSTAR BT21: {packageId: 11539, stickerIds: 52114110-52114149} <br/>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when count is greater than 5.
        /// </exception>
        public List<MessageBase> GetStickerMessages(int packageId, int stickerId, int count = 1) {
            try {
                if (count == 1) {
                    return new List<MessageBase> {
                        new StickerMessage(packageId, stickerId)
                    };
                }

                if (count < 1 || count > 5) {
                    return new List<MessageBase> { GetTextMessage("參數錯誤！（個數必須介於1-5）") };
                }

                var messages = new List<MessageBase>();
                for (int i = 0; i < count; i++) {
                    messages.Add(new StickerMessage(packageId, stickerId++));
                }

                return messages;
            } catch (Exception ex) {
                string errorMsg = $"[GetStickerMessages] " +
                    $"packageId: {packageId}, stickerId: {stickerId}, ex: {ex}";
                Log.Error(errorMsg);
                return new List<MessageBase> { GetTextMessage(errorMsg) };
            }
        }
    }
}