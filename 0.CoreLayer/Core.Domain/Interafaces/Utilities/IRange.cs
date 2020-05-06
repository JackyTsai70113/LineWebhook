using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Interafaces.Utilities {

    public interface IRange<T> {

        /// <summary>
        /// 開始
        /// </summary>
        T Start { get; }

        /// <summary>
        /// 結束
        /// </summary>
        T End { get; }

        /// <summary>
        /// 是否包含指定值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>是否包含</returns>
        bool Includes(DateTime value);

        /// <summary>
        /// 是否包含區間
        /// </summary>
        /// <param name="range">區間</param>
        /// <returns>是否包含</returns>
        bool Includes(IRange<DateTime> range);
    }
}