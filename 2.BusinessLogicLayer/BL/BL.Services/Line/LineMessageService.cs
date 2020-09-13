using System;
using System.Collections.Generic;
using isRock.LineBot;
using Serilog;

namespace BL.Services.Line {
    /// <summary>
    /// 產生需使用的Line Message
    /// </summary>
    public class LineMessageService {

        public List<MessageBase> GetSingleMessage(string text) {
            List<MessageBase> messages = new List<MessageBase>();
            try {
                text = text.Replace('\'', '’').Trim();
                messages.Add(new TextMessage(text));
            } catch (Exception ex) {
                Log.Error($"[GetSingleMessage] text: {text} Exception: {ex}");
            }
            return messages;
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
                    return GetSingleMessage("參數錯誤！（個數必須介於1-5）");
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
                return GetSingleMessage(errorMsg);
            }
        }
    }
}