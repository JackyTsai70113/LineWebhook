using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages {

    /// <summary>
    /// 圖片訊息
    /// </summary>
    public class ImageMessage : Message {

        public ImageMessage() {
            type = "image";
        }

        /// <summary>
        /// 原圖
        /// </summary>
        /// <remarks>
        /// Image URL (Max character limit: 1000)
        /// HTTPS over TLS 1.2 or later
        /// JPG, JPEG, or PNG
        /// Max image size: No limits
        /// Max file size: 10 MB
        /// </remarks>
        public string originalContentUrl { get; set; }

        /// <summary>
        /// 預覽圖
        /// </summary>
        /// <remarks>
        /// Image URL (Max character limit: 1000)
        /// HTTPS over TLS 1.2 or later
        /// JPG, JPEG, or PNG
        /// Max image size: No limits
        /// Max file size: 10 MB
        /// </remarks>
        public string previewImageUrl { get; set; }
    }
}