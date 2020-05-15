using BL.Interfaces.Base;
using Core.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Base {

    public class BaseService : IBaseService {
        protected static readonly string LineWebhookContextConnectionString = ConfigurationUtility.GetSqlConnectionString("LineWebhookContext");

        protected string GetConnectionString(DBContextEnum dBContextEnum) {
            return ConfigurationUtility.GetSqlConnectionString(dBContextEnum.ToString());
        }

        protected enum DBContextEnum {
            LineWebhookContext = 0
        }
    }
}