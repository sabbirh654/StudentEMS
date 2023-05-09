using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace StudentEMS.Services
{
    public class DatabaseAccess : IDatabaseAccess
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public void ExecuteQuery(string query, Dictionary<string, object> parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    AddParametersToCommand(parameters, cmd);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public DataTable GetData(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    AddParametersToCommand(parameters, cmd);

                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    dataAdapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        public string GetSingleData(string query, Dictionary<string, object> parameters = null)
        {
            object dbValue = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    AddParametersToCommand(parameters, cmd);
                    dbValue = cmd.ExecuteScalar();
                }
            }

            return dbValue == null ? null : Convert.ToString(dbValue);
        }
        public void AddParametersToCommand(Dictionary<string, object> parameters, MySqlCommand cmd)
        {
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                }
            }
        }
    }
}