using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Entity
{
    public sealed class SQLHelper
    {
        private SQLHelper() {}

        private readonly static string connStr = "Data Source=OEM-20150906RAM;Initial Catalog=LotteryDB;Integrated Security=True";

        public static int ExecteNonQuery(string sqlStr, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteDataTable(string sqlStr, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }
    }
}