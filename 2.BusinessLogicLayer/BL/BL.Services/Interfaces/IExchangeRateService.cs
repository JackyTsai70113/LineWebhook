using System.Collections.Generic;
using isRock.LineBot;

namespace BL.Services.Interfaces {

    public interface IExchangeRateService {

        /// <summary>
        /// 取得LINE訊息列表
        /// </summary>
        /// <returns>LINE訊息列表</returns>
        List<MessageBase> GetExchangeRateMessage();
    }
}