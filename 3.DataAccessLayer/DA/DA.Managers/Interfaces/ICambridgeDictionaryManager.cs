using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Managers.Interfaces {

    public interface ICambridgeDictionaryManager {

        List<Translation> CrawlCambridgeDictionary(string vocabulary);
    }
}