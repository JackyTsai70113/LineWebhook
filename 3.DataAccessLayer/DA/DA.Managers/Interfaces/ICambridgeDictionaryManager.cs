using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using System.Collections.Generic;

namespace DA.Managers.Interfaces {

    public interface ICambridgeDictionaryManager {

        List<Translation> CrawlCambridgeDictionary(string vocabulary);
    }
}