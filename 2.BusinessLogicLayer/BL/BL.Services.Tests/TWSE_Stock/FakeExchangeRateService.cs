using System;
using System.Collections.Generic;
using System.Text;
using BL.Services.Interfaces;
using Core.Domain.DTO.Sinopac;
using Core.Domain.Utilities;
using isRock.LineBot;

namespace BL.Services.Sinopac {

    public class FakeExchangeRateService : IExchangeRateService {

        /// <summary>
        /// 取得換匯資訊
        /// </summary>
        /// <param name="bankBuyingRate">銀行買入匯率</param>
        /// <param name="bankSellingRate">銀行賣出匯率</param>
        /// <param name="quotedDateTime">報價時間</param>
        public void GetExchangeRate(
            out double bankBuyingRate, out double bankSellingRate,
            out DateTime quotedDateTime) {

            bankBuyingRate = 27.775;
            bankSellingRate = 27.87;
            quotedDateTime = new DateTime(2022, 1, 28, 15, 30, 38);
        }
    }
}