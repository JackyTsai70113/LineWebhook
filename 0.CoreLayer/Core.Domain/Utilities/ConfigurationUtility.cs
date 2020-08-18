using Microsoft.Extensions.Configuration;

namespace Core.Domain.Utilities {

    public static class ConfigurationUtility {
        public static IConfiguration Configuration { get; set; }

        public static string GetSqlConnectionString(string name) {
            return Configuration.GetConnectionString(name);
        }

        public static string RedisConfig {
            get {
                return Configuration.GetSection("RedisConfig").Value;
            }
        }
    }
}