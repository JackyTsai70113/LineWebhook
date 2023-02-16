using System.Collections.Generic;
using DA.Managers.CambridgeDictionary;

namespace DA.Managers.Interfaces
{

    public interface ICambridgeDictionaryManager
    {

        List<Translation> CrawlCambridgeDictionary(string vocabulary);
    }
}