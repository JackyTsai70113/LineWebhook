using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Managers.Interfaces {

    public interface ICambridgeDictionaryManager {

        Core.Domain.DTO.RequestDTO.CambridgeDictionary CrawlCambridgeDictionary(string vocabulary);
    }
}