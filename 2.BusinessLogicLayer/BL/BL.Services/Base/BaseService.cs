using BL.Interfaces.Base;
using Core.Domain.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Base {

    public class BaseService : IBaseService {
        protected static readonly string LineWebhookContextConnectionString = ConfigurationUtility.GetSqlConnectionString("LineWebhookContext");
        protected static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected enum DBContextEnum {
            LineWebhookContext = 0
        }
    }
}