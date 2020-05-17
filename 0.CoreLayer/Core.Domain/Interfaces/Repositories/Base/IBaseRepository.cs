using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Core.Domain.Interfaces.Repositories.Base {

    /// <summary>
    /// Repository基本介面
    /// </summary>
    public interface IBaseRepository {

        /// <summary>
        /// 設定SqlServer連線
        /// </summary>
        /// <param name="isAutoCloseConnection">是否自動關閉連線</param>
        /// <param name="connectionString">連線字串</param>
        void SetSqlConnection(string connectionString, bool isAutoCloseConnection = true);
    }
}