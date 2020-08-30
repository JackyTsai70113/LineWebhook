using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Services {

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

        public static string LineChannelAccessToken {
            get {
                return Configuration.GetSection("Line").GetSection("ChannelAccessToken").Value;
            }
        }

        public static string LineJ_userId {
            get {
                return Configuration.GetSection("Line").GetSection("J_userId").Value;
            }
        }
    }
}