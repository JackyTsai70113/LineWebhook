using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages {

    /// <summary>
    /// 貼圖訊息
    /// </summary>
    public class StickerMessage : Message {

        public StickerMessage() {
            type = "sticker";
        }

        public string stickerId { get; set; }
        public string packageId { get; set; }
        public string stickerResourceType { get; set; }
    }
}