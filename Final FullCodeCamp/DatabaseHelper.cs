using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Centralised database helper used by all roles
    public class DatabaseHelper
    {
        // IMPORTANT: Change TANMAY\\SQLEXPRESS to your SQL Server instance name
        // Common alternatives: .\\SQLEXPRESS  |  localhost\\SQLEXPRESS  |  (local)
        private static string connectionString =
            "Data Source=TANMAY\\SQLEXPRESS;Initial Catalog=APUCodeCampDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // SELECT – returns a DataTable with the query results
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                        foreach (SqlParameter p in parameters)
                            cmd.Parameters.Add(p);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static DataTable ExecuteQuery(string query)
        {
            return ExecuteQuery(query, null);
        }

        // INSERT / UPDATE / DELETE – returns affected row count
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            int rows = 0;
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                        foreach (SqlParameter p in parameters)
                            cmd.Parameters.Add(p);
                    conn.Open();
                    rows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return rows;
        }

        public static int ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, null);
        }

        // Returns a single scalar value e.g. COUNT(*)
        public static object ExecuteScalar(string query, SqlParameter[] parameters)
        {
            object result = null;
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                        foreach (SqlParameter p in parameters)
                            cmd.Parameters.Add(p);
                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        public static object ExecuteScalar(string query)
        {
            return ExecuteScalar(query, null);
        }

        // Quick connection check before the app starts
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
