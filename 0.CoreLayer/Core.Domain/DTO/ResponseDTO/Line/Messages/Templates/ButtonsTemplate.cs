using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates {

    /// <summary>
    /// 按鈕模板
    /// </summary>
    public class ButtonsTemplate : Template {

        public ButtonsTemplate() {
            type = "buttons";
        }

        /// <summary>
        /// 圖片網址 (Max character limit: 1,000)
        /// </summary>
        /// <remarks>
        /// HTTPS over TLS 1.2 or later
        /// JPEG or PNG
        /// Max width: 1024px
        /// Max file size: 1 MB
        /// </remarks>
        public string thumbnailImageUrl { get; set; }

        // rectangle: 1.51:1 (預設)/ square: 1:1
        public string imageAspectRatio { get; set; }

        /// <summary>
        /// 滿版 cover / 包含 contain
        /// </summary>
        /// <remarks>
        /// cover: The image fills the entire image area.
        /// Parts of the image that do not fit in the area are not displayed.
        /// contain: The entire image is displayed in the image area.
        /// A background is displayed in the unused areas to the left and right of vertical images
        /// and in the areas above and below horizontal images.
        /// </remarks>
        public string imageSize { get; set; }

        /// <summary>
        /// 圖片背景顏色, 預設 #FFFFFF (white)
        /// </summary>
        public string imageBackgroundColor { get; set; }

        /// <summary>
        /// 標題, 上限 40 字元
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// [必填] 文字, 上限字元: 160 (無圖或標題) / 60 (有圖或標題)
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 預設點擊動作, 點擊圖片, 標題, 文字的預設動作
        /// </summary>
        public ActionObject defaultAction { get; set; }

        /// <summary>
        /// [必填] 點擊動作列表 (最多四個)
        /// </summary>
        public List<ActionObject> actions { get; set; }
    }
}