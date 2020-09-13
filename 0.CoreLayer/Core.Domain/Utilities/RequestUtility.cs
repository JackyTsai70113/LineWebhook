using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Domain.Utilities {

    public static class RequestUtility {

        private static readonly List<string> uriList = new List<string> {
            "http://144.121.248.114:8080",
            "http://200.255.122.170:8080",
            "http://23.31.218.226:80",
            "http://94.177.247.230:82",
            "http://154.16.202.22:8080",
            "http://46.4.96.137:3128",
            "http://5.252.161.48:8080",
            "http://173.249.24.52:3128",

            "http://80.51.31.201:8080",
            "http://102.67.19.132:8080",
            "http://87.255.28.153:8080",
            "http://109.92.210.250:32231",
            "http://92.247.93.142:43441",
            "http://175.100.16.20:37725",
            "http://124.41.213.44:53931",
            "http://36.89.194.113:38622",
            "http://124.41.213.211:58786",
            "http://103.221.254.125:51630",
            "http://119.82.252.1:41714",
            "http://185.63.46.205:57100",
            "http://119.82.253.24:44060",
            "http://167.99.166.194:8081",
            "http://201.234.185.147:42226",
            "http://186.42.186.202:56569",
            "http://91.207.61.196:1182",
            "http://139.255.94.122:53328",
            "http://195.182.152.238:38178",
            "http://139.255.87.234:35241",
            "http://116.193.221.69:34328",
            "http://103.25.167.200:60862",
            "http://3.0.78.236:80",
            "http://103.126.218.68:8080",
            "http://3.95.11.66:3128",
            "http://207.144.111.230:8080",
            "http://187.111.176.129:8080",
            "http://195.191.250.38:3128",
            "http://180.179.98.22:3128",
            "http://202.154.180.53:48049",
            "http://138.118.224.36:41377",
            "http://118.172.201.32:54059",
            "http://103.216.82.199:6666",
            "http://165.90.209.79:30573",
            "http://173.82.115.174:5836",
            "http://49.128.178.22:45643",
            "http://221.120.210.211:39617",
            "http://109.74.142.138:53281",
            "http://220.135.8.49:40297",
            "http://167.172.161.25:3128",
            "http://78.60.203.75:40494",
            "http://125.26.7.114:30638",
            "http://36.89.182.153:36152",
            "http://103.35.132.50:36555",
            "http://13.234.143.58:443",
            "http://186.159.2.249:38763",
            "http://181.129.140.226:36733",
            "http://185.188.218.10:60928",
            "http://45.71.80.1:43926",
            "http://201.193.180.14:39675",
            "http://89.248.244.182:8080",
            "http://103.77.60.22:55443",
            "http://123.200.20.242:58847",
            "http://78.46.40.154:8118",
            "http://186.248.170.82:53281",
            "http://88.79.82.103:8080",
            "http://186.125.59.8:44363",

            "http://182.48.81.222:38315",
            "http://187.94.31.237:33296",
            "http://31.209.110.159:42275",
            "http://191.7.208.81:50626",
            "http://161.35.78.4:3128",
            "http://201.150.144.102:60691",
            "http://176.58.79.206:8080",
            "http://118.174.232.239:39258",
            "http://119.82.253.123:48405",
            "http://117.206.151.66:42353",
            "http://93.78.238.94:41258",
            "http://165.16.3.57:53281",
            "http://124.41.211.212:23500",
            "http://102.164.203.237:36775",
            "http://144.202.9.4:3128",
            "http://187.178.4.242:34125",
            "http://41.72.203.66:40231",
            "http://91.207.249.253:80",
            "http://216.228.69.202:54085",
            "http://150.129.58.190:31111",
            "http://190.11.15.14:55443",
            "http://200.41.94.97:41310",
            "http://177.94.225.58:36189",
            "http://179.189.52.2:59880",
            "http://202.40.177.69:80",
            "http://192.140.42.81:39292",
            "http://45.251.57.115:8080",
            "http://177.99.206.82:8080",
            "http://186.29.163.97:49787",
            "http://170.82.231.26:56805",
            "http://90.154.223.204:8080",
            "http://103.15.167.124:41787",
            "http://190.152.17.62:55443",
            "http://92.247.142.14:53281",
            "http://41.78.243.194:53281",
            "http://210.56.245.77:8080",
            "http://5.189.133.231:80",
            "http://179.124.240.199:46318",
            "http://103.116.203.242:43520",
            "http://197.210.153.126:32729",
            "http://103.250.158.21:6666",
            "http://81.200.63.108:60579",
            "http://165.16.118.206:8080",
            "http://178.134.208.126:50297",
            "http://139.130.206.73:51376",
            "http://14.38.255.1:80",
            "http://190.166.249.44:37359",
            "http://103.105.212.106:53281",
            "http://203.160.190.250:61161",
            "http://176.235.80.110:9090",
            "http://150.107.22.209:8080",
            "http://197.231.146.160:8080",
            "http://177.93.102.192:8080",
            "http://5.202.192.147:8080",
            "http://199.247.8.250:3128",
            "http://201.210.10.125:8080",
            "http://212.129.58.81:5836",
            "http://109.74.44.97:8080",
            "http://191.10.147.49:8080",
            "http://188.94.229.5:8080",
            "http://45.13.81.240:8080",
            "http://186.219.96.12:45982",
            "http://5.200.81.28:8080",
            "http://203.174.13.158:8082",
            "http://5.160.208.168:8080",
            "http://176.110.157.29:8080",
            "http://121.139.171.31:80",
            "http://109.184.223.232:8080",
            "http://186.96.115.198:8080",
            "http://160.238.250.195:80",
            "http://177.165.158.82:8080",
            "http://154.119.33.92:8080",
            "http://178.159.38.134:8080",
            "http://117.206.151.187:8080",
            "http://179.127.241.158:38927",
            "http://194.58.90.246:3000",
            "http://187.47.36.49:8080",
            "http://2.187.233.106:8080",
            "http://122.102.27.212:23500",
            "http://105.225.249.243:8080",
            "http://103.29.221.53:48025",
            "http://198.58.10.49:8080",
            "http://134.35.77.105:8080",
            "http://177.165.203.193:8080",
            "http://116.58.236.155:8080",
            "http://186.219.96.225:47514",
            "http://125.27.114.206:8080",
            "http://41.217.206.86:8080",
            "http://176.235.148.42:9090",
            "http://103.224.38.2:8080",
            "http://124.122.152.48:8080",
            "http://187.95.27.254:20183",
            "http://138.0.207.18:47719",
            "http://40.83.211.207:80",
            "http://134.119.179.198:5836",
            "http://178.239.151.65:8080",
            "http://134.35.217.4:8080",
            "http://192.200.200.124:3128",
            "http://36.37.114.41:41454",
            "http://37.235.24.98:8080",
            "http://41.66.76.88:8080",
            "http://178.228.9.124:8080",
            "http://36.67.8.245:58626",
            "http://103.15.242.212:47424",
            "http://36.90.106.92:3128",
            "http://152.89.152.51:8080",
            "http://68.183.29.15:8080",
            "http://200.38.19.224:80",
            "http://103.47.93.207:51618",
            "http://165.73.128.125:56975",
            "http://121.52.145.163:8080",
            "http://18.236.114.11:3128",
            "http://162.243.244.206:80",
            "http://103.137.218.1:83",
            "http://154.73.159.253:8585",
            "http://45.71.114.147:999",
            "http://79.101.106.74:30762",
            "http://89.237.33.1:37647",
            "http://95.209.155.91:8080",
            "http://213.163.122.196:8080",
            "http://14.207.45.149:8080",
            "http://123.108.200.217:82",
            "http://175.101.13.125:8080",
            "http://186.227.185.36:3128",
            "http://43.252.236.22:8080",
            "http://36.68.121.66:8080",
            "http://154.119.33.77:8080",
            "http://187.9.212.50:3128",
            "http://79.106.227.245:8080",
            "http://163.172.118.213:5836",
            "http://180.241.196.124:8080",
            "http://202.191.127.170:8080",
            "http://180.245.117.203:8080",
            "http://213.219.232.218:3128",
            "http://103.58.64.1:8080",
            "http://103.114.11.250:8080",
            "http://134.35.154.81:8080",
            "http://194.79.63.134:8080",
            "http://185.85.219.74:61068",
            "http://206.189.184.46:80",
            "http://91.147.64.62:8080",
            "http://104.248.90.212:80",
            "http://51.158.68.68:8811",
            "http://114.43.174.23:8080",
            "http://46.197.209.217:3128",
            "http://163.172.136.226:8811",
            "http://167.172.219.166:3128",
            "http://103.78.75.165:8080",
            "http://85.10.219.100:1080",
            "http://186.219.155.72:61347",
            "http://182.48.94.238:8080",
            "http://5.190.63.115:8080",
            "http://81.144.138.35:3128",
            "http://103.105.77.15:8080",
            "http://23.137.128.122:41549",
            "http://103.94.7.254:53281",
            "http://197.216.2.14:8080",
            "http://161.35.78.63:3128",
            "http://217.219.179.60:5220",
            "http://115.85.67.252:32295",
            "http://64.227.126.28:3128",
            "http://203.223.41.34:57897",
            "http://103.221.254.44:56471",
            "http://197.216.2.18:8080",
            "http://80.65.28.57:30962",
            "http://103.250.68.10:8080",
            "http://91.207.238.107:53007",
            "http://179.127.242.53:30383",
            "http://195.138.72.84:55126",
            "http://212.72.159.22:30323",
            "http://80.237.20.20:52790",
            "http://197.211.238.220:54675",
            "http://91.219.56.221:8080",
            "http://185.162.142.81:53281",
            "http://78.108.66.26:3128",
            "http://118.174.233.40:45976",
            "http://86.125.112.230:57373",
            "http://191.37.183.209:60139",
            "http://46.35.249.189:60066",
            "http://85.223.157.204:40329",
            "http://193.33.241.61:45837",
            "http://1.0.191.123:8080",
            "http://198.50.214.17:8585",
            "http://186.0.176.147:80",
            "http://89.40.48.186:8080",
            "http://167.71.198.204:8080",
            "http://77.238.79.111:8080",
            "http://64.227.126.95:3128",
            "http://110.39.187.50:49850",
            "http://195.138.83.218:53281",
            "http://24.172.34.114:49116",
            "http://118.69.140.108:53281",
            "http://109.167.40.129:51085",
            "http://46.229.187.169:53281",
            "http://103.126.13.10:44975",
            "http://41.254.44.241:58690",
            "http://51.68.90.232:9090",
            "http://107.190.148.202:50854",
            "http://85.10.219.99:1080",
            "http://170.238.252.162:50882",
            "http://103.255.53.98:47882",
            "http://176.28.75.229:51357",
            "http://201.73.143.35:8080",
            "http://164.77.147.93:53281",
            "http://200.150.86.138:44677",
            "http://91.192.2.168:53281",
            "http://168.228.192.13:55619",
            "http://200.105.215.18:33630",
            "http://161.35.68.165:3128",
            "http://109.251.185.20:41616",
            "http://212.126.102.142:45212",
            "http://186.47.82.6:41430",
            "http://103.241.227.98:6666",
            "http://123.49.49.166:23500",
            "http://109.167.226.107:38608",
            "http://118.175.93.189:49774",
            "http://80.240.24.119:31444",
            "http://41.78.243.233:53281",
            "http://46.4.96.67:80",
            "http://1.10.186.114:32577",
            "http://190.90.156.126:80"
        };

        private static int uriIndex = 0;
        private static readonly object lockObj = new object();

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
                //httpResponseMessage.EnsureSuccessStatusCode();

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
        /// <param name="uri">Request網址</param>
        /// <returns>stream</returns>
        public static Stream GetStreamFromGetRequest(string uri) {
            try {
                HttpClient client = new HttpClient();

                //發送請求
                HttpResponseMessage httpResponseMessage = client.GetAsync(uri).Result;

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                httpResponseMessage.EnsureSuccessStatusCode();

                return httpResponseMessage.Content.ReadAsStreamAsync().Result;
            } catch (Exception ex) {
                return GetStreamFromGetRequest(uri);
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

        #region HttpClient

        public static HttpClient GetNewHttpClient() {
            // Create a proxy object
            WebProxy proxy = new WebProxy {
                Address = new Uri(uriList[uriIndex]),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
            };

            HttpClientHandler httpClientHandler = new HttpClientHandler {
                Proxy = proxy,
            };
            return new HttpClient(handler: httpClientHandler, disposeHandler: true);
        }

        #endregion HttpClient
        #region Lock

        public static void AddUriIndex() {
            lock (lockObj) {
                uriIndex++;
                uriIndex %= 240;
            }
        }

        #endregion Lock
    }
}