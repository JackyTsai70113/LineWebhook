using System;
using System.Collections.Generic;
using BL.Services.Holiday;
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

        public List<MessageBase> GetListOfSingleMessage(string text) {
            try {
                text = text.Replace('\'', '’').Trim();
                return new List<MessageBase>{
                    GetTextMessage(text)
                };
            } catch (Exception ex) {
                string errorMsg = $"[GetSingleMessage] text: {text} Exception: {ex}";
                Log.Error(errorMsg);
                errorMsg = errorMsg.Replace('\'', '’').Trim();
                return new List<MessageBase>{
                    GetTextMessage(errorMsg)
                };
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>所有Column必須數量相同</remarks>
        public MessageBase GetCarouselTemplateMessage() {
            var dates = HolidayHelper.GetTheMostRecentBusinessDay(2);
            var columns = new List<Column> {
                new Column() {
                    thumbnailImageUrl = new Uri("https://i.imgur.com/n82BOcq.png"),
                    title = "買賣超彙",
                    text = "計算外資及陸資，投信綜合買賣超彙\n" +
                        "請設定計算區間:",
                    actions = new List<TemplateActionBase> {
                        new PostbackAction {
                            label = "一天內👉",
                            data = "tv 1",
                            displayText = "我要查詢一天內綜合買賣超彙🙏"
                        },
                        new PostbackAction {
                            label = "三天內👉",
                            data = "tv 3",
                            displayText = "我要查詢三天內綜合買賣超彙🙏"
                        },
                        new PostbackAction {
                            label = "五天內👉",
                            data = "tv 5",
                            displayText = "我要查詢五天內綜合買賣超彙🙏"
                        }
                    }
                },
                new Column() {
                    thumbnailImageUrl = new Uri("https://i.imgur.com/n82BOcq.png"),
                    title = "買賣超彙",
                    text = "計算外資及陸資，投信綜合買賣超彙\n" +
                        "請選擇日期:",
                    actions = new List<TemplateActionBase> {
                        new PostbackAction {
                            label = $"{dates[0]:yyyy/MM/dd}👉",
                            data = "tv " + dates[0].ToString("yyyyMMdd"),
                            displayText = $"我要查詢{dates[0]:yyyy/MM/dd}的綜合買賣超彙🙏"
                        },
                        new PostbackAction {
                            label = $"{dates[1]:yyyy/MM/dd}👉",
                            data = "tv " + dates[1].ToString("yyyyMMdd"),
                            displayText = $"我要查詢{dates[1]:yyyy/MM/dd}的綜合買賣超彙🙏"
                        },
                        new DateTimePickerAction {
                            label = "選擇日期👉",
                            data = "DateTimePickerAction_data",
                            mode = "date",
                            initial = DateTime.UtcNow.AddHours(8).Date.ToString("yyyy-MM-dd"),
                            max = new DateTime(2025, 12, 31).ToString("yyyy-MM-dd"),
                            min = new DateTime(2011, 1, 1).ToString("yyyy-MM-dd")
                        }
                    }
                },
            };
            return new TemplateMessage(new CarouselTemplate() { columns = columns });
        }

        public MessageBase GetTextMessageWithQuickReply() {
            var quickReply = new QuickReply {
                items = new List<QuickReplyItemBase>{
                                    new QuickReplyMessageAction("一天內", "tv 1") {
                                        imageUrl = new Uri("https://imgur.com/ZQVKq9T.png"),
                                    },
                                    new QuickReplyMessageAction("三天內", "tv 3") {
                                        imageUrl = new Uri("https://imgur.com/ZQVKq9T.png"),
                                    },
                                    new QuickReplyMessageAction("五天內", "tv 5") {
                                        imageUrl = new Uri("https://imgur.com/ZQVKq9T.png"),
                                    },
                                    new QuickReplyMessageAction("查詢指定日期", "tv date") {
                                        imageUrl = new Uri("https://imgur.com/ZQVKq9T.png"),
                                    },
                                }
            };
            return new TextMessage("開始統計買賣超彙 請問計算區間為何?") { quickReply = quickReply };
        }

        public StickerMessage GetStickerMessage(Message stickerMessage) {
            int packageId = stickerMessage.packageId;
            int stickerId = stickerMessage.stickerId;
            return new StickerMessage(packageId, stickerId);
        }

        /// <summary>
        /// 取得貼圖訊息
        /// </summary>
        /// <remark>
        /// https://developers.line.biz/media/messaging-api/sticker_list.pdf
        /// https://devdocs.line.me/files/sticker_list.pdf
        /// 範圍包含：
        /// Moon James              1              1-17    21 100-139 401-430
        /// Brown Cony              2             18-20 22-47 140-179 501-527
        /// Cherry coco             3           180-259
        /// Daily Life              4           260-307 601-632
        /// Brown, Cony & Sally 11537 52002734-52002773
        /// CHOCO & Friends     11538 51626494-51626533
        /// UNIVERSTAR BT21     11539 52114110-52114149
        /// </remark>
        /// <exception cref="System.ArgumentOutOfRangeException">
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
                    return GetListOfSingleMessage("參數錯誤！（個數必須介於1-5）");
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
                return GetListOfSingleMessage(errorMsg);
            }
        }
    }
}