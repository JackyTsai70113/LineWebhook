using System;
using System.Collections.Generic;
using System.Text;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.DTO.Sinopac;

namespace DA.Managers.Interfaces.Sinopac {

    public interface IExchangeRateManager {

        List<ExchangeRate> CrawlExchangeRate();
    }
}