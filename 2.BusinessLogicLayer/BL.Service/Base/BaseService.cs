using BL.Service.Interface.Base;
using Core.Domain.Utilities;

namespace BL.Service.Base {

    public class BaseService : IBaseService {
        protected static readonly string LineWebhookContextConnectionString = ConfigurationUtility.GetSqlConnectionString("LineWebhookContext");
        protected static readonly string TestDBConnectionString = ConfigurationUtility.GetSqlConnectionString("TestDBContext");
    }
}