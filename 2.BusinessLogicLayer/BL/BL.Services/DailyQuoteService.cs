using Core.Domain.Interafaces.Repositories;
using Core.Domain.Interafaces.Services;
using DA.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services {

    public class DailyQuoteService : IDailyQuoteService {

        public DailyQuoteService() {
            DailyQuoteRepository = new DailyQuoteRepository();
        }

        public IDailyQuoteRepository DailyQuoteRepository { get; set; }

        public void CreateDailyQuote() {
            DailyQuoteRepository.InsertDailyQuote();
        }
    }
}