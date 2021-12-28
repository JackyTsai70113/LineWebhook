using Core.Domain.Utilities;
using ExcelDataReader;
using ExcelDataReader.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BL.Services.Excel {

    public class ExcelDataReaderService {

        public ExcelDataReaderService() {
        }

        public DataSet GetDataSetFromUri() {
            var result = new DataSet();
            //var stream = RequestUtility.GetStreamFromGetRequest(uri);
            //using (var reader = ExcelReaderFactory.CreateCsvReader(
            //    stream, new ExcelReaderConfiguration() { FallbackEncoding = Encoding.GetEncoding("big5") })) {
            //    // Choose one of either 1 or 2:

            //    // 1. Use the reader methods
            //    do {
            //        while (reader.Read()) {
            //            // reader.GetDouble(0);
            //        }
            //    } while (reader.NextResult());

            //    // 2. Use the AsDataSet extension method
            //    result = reader.AsDataSet();
            //}
            return result;
        }
    }
}