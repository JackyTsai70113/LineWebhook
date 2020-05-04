using Core.Domain.Interafaces.Repositories.Base;
using Core.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DA.Repositories.Base {

    public class BaseRepository : IBaseRepository {
        public static SqlConnection SqlConnection { get; set; }

        public BaseRepository() {
            SqlConnection = new SqlConnection(ConfigurationUtility.GetSqlConnection());
        }
    }
}