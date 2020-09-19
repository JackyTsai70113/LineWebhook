using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services {

    public class ConfigService {
        public static IConfiguration Configuration { get; set; }

        public static string GetSqlConnectionString(string name) {
            return Configuration.GetConnectionString(name);
        }

        public static string Redis_Endpoint {
            get {
                return Configuration.GetSection("Redis").GetSection("Endpoint").Value;
            }
        }

        public static string Redis_Password {
            get {
                return Configuration.GetSection("Redis").GetSection("Password").Value;
            }
        }

        public static string Line_ChannelAccessToken {
            get {
                return Configuration.GetSection("Line").GetSection("ChannelAccessToken").Value;
            }
        }

        public static string Line_Jacky_userId {
            get {
                return Configuration.GetSection("Line").GetSection("Jacky_userId").Value;
            }
        }

        public static string Line_Jessi_userId {
            get {
                return Configuration.GetSection("Line").GetSection("Jessi_userId").Value;
            }
        }

        public static string Line_NotifyBearerToken_Group {
            get {
                return Configuration.GetSection("Line").GetSection("NotifyBearerToken_Group").Value;
            }
        }

        public static string Line_NotifyBearerToken_Jacky {
            get {
                return Configuration.GetSection("Line").GetSection("NotifyBearerToken_Jacky").Value;
            }
        }

        public static string Line_NotifyBearerToken_Jessi {
            get {
                return Configuration.GetSection("Line").GetSection("NotifyBearerToken_Jessi").Value;
            }
        }

        public static string MapQuest_Key {
            get {
                return Configuration.GetSection("MapQuest_Key").Value;
            }
        }

        public static string HereApi_Key {
            get {
                return Configuration.GetSection("HereApi_Key").Value;
            }
        }

        public static int TWSE_TradingVolumeNumber {
            get {
                string numberStr = Configuration.GetSection("TWSE").GetSection("TradingVolumeNumber").Value;
                return int.Parse(numberStr);
            }
        }
    }
}