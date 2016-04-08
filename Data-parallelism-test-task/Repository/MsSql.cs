using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ConcurrencyTest.Repository {
    public class MsSql {
        public static SqlCommand NewCommand(SqlConnection cn, string ProcName) {
            var cmd = new SqlCommand(ProcName, cn) { CommandType = CommandType.StoredProcedure };
            SqlCommandBuilder.DeriveParameters(cmd);
            return cmd;
        }
    }
}