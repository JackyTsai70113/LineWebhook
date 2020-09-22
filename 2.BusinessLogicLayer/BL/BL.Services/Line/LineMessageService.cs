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

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>所有Column必須數量相同</remarks>
        public MessageBase GetCarouselTemplateMessage(string ascOrDesc) {
            string chineseWord, command;
            if (ascOrDesc == "desc") {
                chineseWord = "買超";
                command = "tv";
            } else if (ascOrDesc == "asc") {
                chineseWord = "賣超";
                command = "tvv";
            } else {
                throw new ArgumentException("ascOrDesc 不合法", "ascOrDesc");
            }
            var dates = HolidayHelper.GetTheMostRecentBusinessDay(2);
            var columns = new List<Column> {
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