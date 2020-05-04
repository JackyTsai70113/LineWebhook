using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Utilities {

    public static class ConfigurationUtility {
        public static IConfiguration Configuration { get; set; }

        public static string GetSqlConnection() {
            return Configuration.GetConnectionString("LineWebhookContext");
        }
    }
}