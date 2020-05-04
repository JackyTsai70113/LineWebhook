using Core.Domain.Entities.TWSE_Stock.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Interafaces.Repositories {

    public interface IDailyQuoteRepository {

        void InsertDailyQuote(DailyQuote dailyQuote = null);
    }
}