using System;

namespace BL.Services.Interfaces {

    public interface IExchangeRateService {

        /// <summary>
        /// 取得換匯資訊
        /// </summary>
        /// <param name="bankBuyingRate">銀行買入匯率</param>
        /// <param name="bankSellingRate">銀行賣出匯率</param>
        /// <param name="quotedDateTime">報價時間</param>
        void GetExchangeRate(out double bankBuyingRate, out double bankSellingRate,
            out DateTime quotedDateTime);
    }
}