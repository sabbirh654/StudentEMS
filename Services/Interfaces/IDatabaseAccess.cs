using System.Collections.Generic;
using System.Data;

using MySql.Data.MySqlClient;

namespace StudentEMS
{
    public interface IDatabaseAccess
    {
        void ExecuteQuery(string query, Dictionary<string, object> parameters);
        DataTable GetData(string query, Dictionary<string, object> parameters = null);
        string GetSingleData(string query, Dictionary<string, object> parameters = null);
        void AddParametersToCommand(Dictionary<string, object> parameters, MySqlCommand cmd);
    }
}
