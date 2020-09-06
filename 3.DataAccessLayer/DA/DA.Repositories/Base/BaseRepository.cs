using DA.Repositories.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace DA.Repositories.Base {

    public class BaseRepository<T> : IBaseRepository where T : new() {
        private T TObject { get; set; } = new T();

        /// <summary>
        /// 是否自動關閉連線
        /// </summary>
        private static bool IsAutoCloseConnection { get; set; }

        /// <summary>
        /// Sql Server連線
        /// </summary>
        private static SqlConnection SqlConnection { get; set; }

        /// <summary>
        /// 取得Sql Server連線
        /// </summary>
        /// <returns>Sql Server連線</returns>
        public static SqlConnection GetSqlConnection() {
            return SqlConnection;
        }

        /// <summary>
        /// 設定Sql Server連線
        /// </summary>
        /// <param name="isAutoCloseConnection">是否自動關閉連線</param>
        /// <param name="connectionString">連線字串</param>
        public void SetSqlConnection(string connectionString, bool isAutoCloseConnection = true) {
            IsAutoCloseConnection = isAutoCloseConnection;
            SqlConnection = new SqlConnection(connectionString);
        }

        public static void OpenSqlConnection() {
            if (SqlConnection.State != ConnectionState.Open) {
                SqlConnection.Open();
            }
        }

        public static void CloseConnection() {
            if (SqlConnection.State != ConnectionState.Closed) {
                SqlConnection.Close();
            }
        }

        private static void AutoOpenConnection() {
            if (IsAutoCloseConnection) {
                OpenSqlConnection();
            }
        }

        private static void AutoCloseConnection() {
            if (IsAutoCloseConnection) {
                CloseConnection();
            }
        }

        public int Execute(string cmdText, object obj) {
            try {
                AutoOpenConnection();
                int affectedRowNumber = SqlConnection.Execute(cmdText, obj);
                return affectedRowNumber;
            } catch (Exception ex) {
                Console.WriteLine(
                    $"QueryFirst 失敗, " +
                    $"cmdText: {cmdText}, " +
                    $"obj: {obj}" +
                    $"ex: {ex}");
                return default;
            } finally {
                AutoCloseConnection();
            }
        }

        public T QueryFirst(string cmdText) {
            try {
                T result;
                AutoOpenConnection();

                SqlCommand sqlCommand = new SqlCommand(cmdText, SqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                result = MapSqlDataReader(sqlDataReader);

                return result;
            } catch (Exception ex) {
                Console.WriteLine(
                    $"QueryFirst 失敗, " +
                    $"cmdText: {cmdText}, " +
                    $"ex: {ex}");
                return default;
            } finally {
                AutoCloseConnection();
            }
        }

        public T QueryFirst(string cmdText, object obj) {
            try {
                T result;
                AutoOpenConnection();

                result = SqlConnection.QueryFirst<T>(cmdText, obj);

                return result;
            } catch (Exception ex) {
                Console.WriteLine(
                    $"QueryFirst 失敗, " +
                    $"cmdText: {cmdText}, " +
                    $"ex: {ex}");
                return default;
            } finally {
                AutoCloseConnection();
            }
        }

        /// <summary>
        /// 根據 SQL字串，條件物件 取得 Enumerable<T>
        /// </summary>
        /// <param name="cmdText">SQL字串</param>
        /// <param name="obj">條件物件</param>
        /// <returns>Enumerable<T></returns>
        public IEnumerable<T> QueryEnumerable(string cmdText, object obj) {
            try {
                IEnumerable<T> result;
                AutoOpenConnection();

                result = SqlConnection.Query<T>(cmdText, obj);

                return result;
            } catch (Exception ex) {
                Console.WriteLine(
                    $"QueryFirst 失敗, " +
                    $"cmdText: {cmdText}, " +
                    $"ex: {ex}");
                return default;
            } finally {
                AutoCloseConnection();
            }
        }

        #region 處理SqlDataReader

        private T MapSqlDataReader(IDataReader dataReader) {
            T TResult = TObject;
            if (dataReader.Read()) {
                PropertyInfo[] propertyInfos = TResult.GetType().GetProperties();
                foreach (var pi in propertyInfos) {
                    object valueObject = dataReader[pi.Name];
                    pi.SetValue(TResult, ParseToGetValue(valueObject), null);
                }
            }

            return TResult;
        }

        private static object ParseToGetValue(object data) {
            try {
                string typeName = data.GetType().Name;
                switch (typeName) {
                    case "String":
                        return data.ToString();

                    case "Int32":
                        return int.Parse(data.ToString());

                    case "Single":
                        return float.Parse(data.ToString());

                    case "decimal":
                        return decimal.Parse(data.ToString());

                    case "float":
                        return float.Parse(data.ToString());

                    case "DateTime":
                        return DateTime.Parse(data.ToString());

                    default:
                        throw new Exception("Not Found!");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        #endregion 處理SqlDataReader
    }
}