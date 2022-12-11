using Microsoft.Extensions.Configuration;

namespace Website {

    public class Config {
        private static IConfiguration _configuration { get; set; }
        public Config(IConfiguration configuration) {
            _configuration = configuration;
        }

        public static string Redis_Endpoint => _configuration.GetSection("Redis").GetSection("Endpoint").Value;

        public static string Redis_Password => _configuration.GetSection("Redis").GetSection("Password").Value;

        public static string Line_ChannelAccessToken => _configuration.GetSection("Line").GetSection("ChannelAccessToken").Value;

        public static string Line_Jacky_userId => _configuration.GetSection("Line").GetSection("Jacky_userId").Value;

        public static string Line_Jessi_userId => _configuration.GetSection("Line").GetSection("Jessi_userId").Value;

        public static string Line_NotifyBearerToken_Group => _configuration.GetSection("Line").GetSection("NotifyBearerToken_Group").Value;

        public static string Line_NotifyBearerToken_Jacky => _configuration.GetSection("Line").GetSection("NotifyBearerToken_Jacky").Value;

        public static string Line_NotifyBearerToken_Jessi => _configuration.GetSection("Line").GetSection("NotifyBearerToken_Jessi").Value;

        public static string MapQuest_Key => _configuration.GetSection("MapQuest_Key").Value;

        public static string HereApi_Key => _configuration.GetSection("HereApi_Key").Value;

        public static int TWSE_TradingVolumeNumber {
            get {
                string numberStr = _configuration.GetSection("TWSE").GetSection("TradingVolumeNumber").Value;
                return int.Parse(numberStr);
            }
        }
    }
}