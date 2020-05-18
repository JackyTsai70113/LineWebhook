using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates {

    public class CarouselTemplate : Template {

        public CarouselTemplate() {
            type = "carousel";
        }

        /// <summary>
        /// Max columns: 10
        /// </summary>
        public List<ColumnObject> columns { get; set; }

        // rectangle: 1.51:1 / square: 1:1
        public string imageAspectRatio { get; set; }

        // cover / contain
        /*
         cover: The image fills the entire image area. Parts of the image that do not fit in the area are not displayed.
        contain: The entire image is displayed in the image area. A background is displayed in the unused areas to the left and right of vertical images and in the areas above and below horizontal images.
        */
        public string imageSize { get; set; }
    }

    public class ColumnObject {

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Image URL (Max character limit: 1,000)
        /// HTTPS over TLS 1.2 or later
        /// JPEG or PNG
        /// Max width: 1024px
        /// Max file size: 1 MB
        /// </remarks>
        public string thumbnailImageUrl { get; set; }

        // rectangle: 1.51:1 / square: 1:1
        public string imageAspectRatio { get; set; }

        // cover / contain
        /*
         cover: The image fills the entire image area. Parts of the image that do not fit in the area are not displayed.
        contain: The entire image is displayed in the image area. A background is displayed in the unused areas to the left and right of vertical images and in the areas above and below horizontal images.
        */
        public string imageSize { get; set; }

        /// <summary>
        /// Default: #FFFFFF (white)
        /// </summary>
        public string imageBackgroundColor { get; set; }

        /// <summary>
        /// Max character limit: 40
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// title
        /// </summary>
        /// <remarks>
        /// Message text
        /// Max character limit: 160 (no image or title)
        /// Max character limit: 60 (message with an image or title)
        /// </remarks>
        public string text { get; set; }

        /// <summary>
        /// Action when image, title or text area is tapped.
        /// </summary>
        public ActionObject defaultAction { get; set; }

        /// <summary>
        /// Action when tapped
        /// Max objects: 4
        /// </summary>
        public List<ActionObject> actions { get; set; }
    }
}