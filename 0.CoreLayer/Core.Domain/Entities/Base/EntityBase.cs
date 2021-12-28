using Core.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities.Base {

    public abstract class EntityBase {

        public EntityBase() {
            CreateDateTime = DateTimeUtility.Unix_Epoch_StartTime;
            UpdateDateTime = DateTime.Now;
        }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
    }
}