using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Core.Domain.Entities.TestDB;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {

    public class MssqlController : Controller {

        private readonly string connStr = "Server=localhost;Database=master;User Id=sa;Password=Passw0rd1";

        public IActionResult Index() {
            /*
            create store procedure:
                CREATE PROCEDURE SelectInventories @Name nvarchar(30)
                AS
                    SELECT * FROM Inventory WHERE Name = @Name;
                GO;
            */
            List<Inventory> results;
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("dbo.SelectInventories", con)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar, 500).Value = "banana";
                var sql = $"EXEC SelectInventories @Name = '{cmd.Parameters["@Name"].Value}';";
                results = con.Query<Inventory>(sql).ToList();
            }
            return Ok(results);
        }
    }
}
