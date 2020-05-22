using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates {

    public class ImageCarouselTemplate : Template {

        public ImageCarouselTemplate() {
            type = "image_carousel";
        }

        /// <summary>
        /// Column列表, 上限 10 個
        /// </summary>
        public List<ColumnObject> columns { get; set; }
    }
}