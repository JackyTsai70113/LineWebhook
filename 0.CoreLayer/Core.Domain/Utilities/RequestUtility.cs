using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Utilities {

    public static class RequestUtility {

        /// <summary>
        /// 從 GET Request 取得 byte array
        /// </summary>
        /// <param name="uri">Request網址</param>
        /// <returns>byte array</returns>
        public static byte[] GetByteArrayFromGetRequest(string uri) {
            try {
                HttpClient client = new HttpClient();

                //發送請求
                HttpResponseMessage httpResponseMessage = client.GetAsync(uri).Result;

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                httpResponseMessage.EnsureSuccessStatusCode();

                byte[] result = httpResponseMessage.Content.ReadAsByteArrayAsync().Result;
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 從 GET Request 取得字串
        /// </summary>
        /// <param name="uri">Request網址</param>
        /// <returns>字串</returns>
        public static string GetStringFromGetRequest(string uri) {
            try {
                HttpClient client = new HttpClient();

                //發送請求
                HttpResponseMessage httpResponseMessage = client.GetAsync(uri).Result;

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                httpResponseMessage.EnsureSuccessStatusCode();

                string result = httpResponseMessage.Content.ReadAsStringAsync().Result;//取得內容
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 從 GET Request 取得 stream
        /// </summary>
        /// <param name="url">Request網址</param>
        /// <returns>stream</returns>
        public static Stream GetStreamFromGetRequest(string url) {
            try {
                HttpClient client = new HttpClient();

                //發送請求
                HttpResponseMessage httpResponseMessage = client.GetAsync(url).Result;

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                httpResponseMessage.EnsureSuccessStatusCode();

                Stream stream = httpResponseMessage.Content.ReadAsStreamAsync().Result;
                return stream;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 從 GET Request 取得資料流[非同步]
        /// </summary>
        /// <param name="url">Request網址</param>
        /// <returns>資料流</returns>
        public static async Task<Stream> GetStreamFromGetRequestAsync(string url) {
            try {
                HttpClient client = new HttpClient();

                //發送請求
                HttpResponseMessage httpResponseMessage = await client.GetAsync(url);

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                httpResponseMessage.EnsureSuccessStatusCode();

                Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return stream;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}