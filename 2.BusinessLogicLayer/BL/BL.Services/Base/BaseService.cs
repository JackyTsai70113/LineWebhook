using BL.Interfaces.Base;
using Core.Domain.Utilities;

namespace BL.Services.Base {

    public class BaseService : IBaseService {
        protected static readonly string LineWebhookContextConnectionString = ConfigurationUtility.GetSqlConnectionString("LineWebhookContext");

        protected enum DBContextEnum {
            LineWebhookContext = 0
        }
    }
}