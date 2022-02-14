using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;

namespace SCHCINEMA
{
    class ConnectDB
    {
        private static string datasourcestr = "(DESCRIPTION= (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orcl)));";
        private static string connectionStr = "User ID=ID; Password=Password; Data Source=" + datasourcestr; //oracle 서버 연결 (아이디, 비번 설정)
        private string connectSqlStr = "User ID=ID; Password=Password; Server=localhost; Initial Catalog=orcl;"; //SqlConnection용 서버 연결
        private OracleConnection conn;
        private SqlConnection sqlconn;
        private OracleCommand cmd;

        private void ConnectionOracle()
        { 
            try
            {
                if (conn == null)
                {
                    conn = new OracleConnection(connectionStr);
                }
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : The Database Connected is fail..." + e.Message);
            }
        }

        internal void DisconnectionOracle()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        internal void ConnectionSql()
        {
            try {
                sqlconn = new SqlConnection(connectSqlStr);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : The Database Connected is fail..." + e.Message);
            }
        }
        internal void QueryOracle(string query)
        {
            this.ConnectionOracle();

            if (conn == null)
            {
                Console.WriteLine("Error : syntex error");
                return;
            }
            try
            {
                if (cmd == null)
                {
                    cmd = new OracleCommand();
                }
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                Console.WriteLine("success query");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }
            finally
            {
                this.DisconnectionOracle();
            }
        }

        // SELECT 연산을 수행해 결과 값을 반환하는 Query
        internal void SelectQueryOracle(string query, out OracleDataReader result )
        {
            this.ConnectionOracle();
            try
            {
                if (conn == null)
                {
                    Console.WriteLine("Error : syntex error");
                }
                if (cmd == null)
                {
                    cmd = new OracleCommand();
                }
                cmd.Connection = conn;
                cmd.CommandText = query;
                OracleDataReader reader = cmd.ExecuteReader();
                result = reader;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                result = null;
            }
            
        }
        internal void QueryOracle_WithImage(string query, byte[] bytes)
        {
            this.ConnectionOracle();

            if (conn == null)
            {
                Console.WriteLine("Error : syntex error");
                return;
            }
            try
            {
                if (cmd == null)
                {
                    cmd = new OracleCommand();
                }
                OracleParameter para = new OracleParameter();
                para.ParameterName = ":image";
                para.OracleDbType = OracleDbType.Blob;
                para.Direction = ParameterDirection.Input;
                para.SourceColumn = "movie_poster";
                para.Size = bytes.Length;
                para.Value = bytes;
                cmd.Parameters.Add(para);

                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                Console.WriteLine("success query");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }
            finally
            {
                this.DisconnectionOracle();
            }
        }
        internal void getDataset(string query, DataSet DS, string DSname)
        {
            this.ConnectionOracle();
            try
            {
                OracleDataAdapter adapter = new OracleDataAdapter(query, conn);
                adapter.Fill(DS, DSname);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }
            finally
            {
                this.DisconnectionOracle();
            }
        }
    }
    
}
