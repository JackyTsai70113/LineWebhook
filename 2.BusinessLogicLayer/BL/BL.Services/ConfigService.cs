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

        public static string RedisConfig {
            get {
                return Configuration.GetSection("RedisConfig").Value;
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

        public static string Line_NotifyBearerToken_Jacky {
            get {
                return Configuration.GetSection("Line").GetSection("NotifyBearerToken_Jacky").Value;
            }
        }

        public static string Line_NotifyBearerToken_Group {
            get {
                return Configuration.GetSection("Line").GetSection("NotifyBearerToken_Group").Value;
            }
        }
    }
}