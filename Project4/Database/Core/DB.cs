using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Project4.Database.Core
{
    public class DB
    {
        private static DB _instance;
        private readonly MySqlConnection _connection;

        private const string Server = "192.168.178.10";
        private const string Port = "3306";
        private const string Database = "project4";
        private const string User = "root";
        private const string Password = "admin";

        private string ConnectionString;

        public static DB Get()
        {
            return _instance ?? (_instance = new DB());
        }

        private DB()
        {
            var connectionString = new StringBuilder();
            connectionString.Append("Server=" + Server + ";");
            connectionString.Append("Port=" + Port + ";");
            connectionString.Append("Database=" + Database + ";");
            connectionString.Append("Uid=" + User + ";");
            connectionString.Append("Pwd=" + Password + ";");

            ConnectionString = connectionString.ToString();
            _connection = new MySqlConnection(ConnectionString);
        }

        public void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public bool TestConnection()
        {
            MySqlConnection testConnection = null;
            try
            {
                testConnection = new MySqlConnection(ConnectionString);
                testConnection.Open();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                testConnection?.Close();
            }
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }
    }
}