using StudentEMS.Constants;
using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace StudentEMS.Services
{
    public class AuditLogServices : IAuditLogServices
    {
        private IDatabaseHelper _databaseHelper = App.ServiceProvider.GetService<IDatabaseHelper>();
        private IDatabaseAccess _databaseAccess = App.ServiceProvider.GetService<IDatabaseAccess>();
        private static string InsertAuditLogQuery =>
            @"
            INSERT INTO auditlogs(
                UserId,
                Action,
                Description
            )VALUES(
                @userId,
                @action,
                @description
            )";
        private static string GetFullNameQuery =>
            @"
            SELECT 
                FirstName,
                LastName
            FROM users
            WHERE
                UserName = @userName;
            ";

        private static string GetAuditLogsInfoQuery =>
            @"
            SELECT 
                u.Id AS userId,
                u.UserName AS userName,
                u.FirstName AS firstName, 
                u.LastName AS lastName,
                u.Email AS email,
                u2.Name AS userRole,
                a.ACTION AS actionType,
                a.Description AS description,
                a.Timestamp AS timestamp  
            FROM 
                users u
            JOIN 
                auditlogs a 
            ON 
                u.Id = a.UserId
            JOIN 
                userroles u2
            ON 
                u2.Id = u.RoleId
            WHERE 
                 @searchText = '' OR
                (
                    a.Description LIKE @searchText OR 
                    u.UserName LIKE @searchText
                )
            ORDER BY a.Timestamp DESC
            LIMIT @offset,@limit";

        private static string GetAuditLogsCountQuery =>
            @"
            SELECT 
                COUNT(a.Id)
            FROM 
                users u
            JOIN 
                auditlogs a 
            ON 
                u.Id = a.UserId
            JOIN 
                userroles u2
            ON 
                u2.Id = u.RoleId
            WHERE 
                 @searchText = '' OR
                (
                    a.Description LIKE @searchText OR 
                    u.UserName LIKE @searchText
                )";

        public void GenerateAuditLog(int actionType, string userName, string actionDescription)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", _databaseHelper.GetUserId(userName));
            parameters.Add("action", actionType);
            parameters.Add("description", actionDescription);

            try
            {
                _databaseAccess.ExecuteQuery(InsertAuditLogQuery, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public string GetFullName(string userName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userName", userName);

            DataTable dataTable = _databaseAccess.GetData(GetFullNameQuery, parameters);
            Student student = new Student();
            DataRow row = dataTable.Rows[0];
            string fullName = row["FirstName"].ToString() + " " + row["LastName"].ToString();
            return fullName;
        }

        public List<AuditLog> GetAuditLogsInfo(int limit, int offset, string filterText)
        {
            List<AuditLog> auditLogs = new List<AuditLog>();

            var parameters = new Dictionary<string, object>
            {
                {"limit", limit },
                {"offset", offset },
                { "searchText", "%" + filterText + "%"}
            };

            DataTable dataTable = _databaseAccess.GetData(GetAuditLogsInfoQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                AuditLog auditLog = new AuditLog();

                auditLog.UserId = Convert.ToInt32(row["userId"]);
                auditLog.UserName = row["userName"].ToString();
                auditLog.FirstName = row["firstName"].ToString();
                auditLog.LastName = row["lastName"].ToString();
                auditLog.Email = row["email"].ToString();
                auditLog.UserType = row["userRole"].ToString();
                auditLog.Description = row["description"].ToString();
                auditLog.Timestamp = Convert.ToDateTime(row["timestamp"]);

                int actionType = Convert.ToInt32(row["actionType"].ToString());

                switch (actionType)
                {
                    case (int)Constant.AuditLogAction.Add:
                        auditLog.Action = "Add";
                        break;
                    case (int)Constant.AuditLogAction.Update:
                        auditLog.Action = "Update";
                        break;
                    case (int)Constant.AuditLogAction.Delete:
                        auditLog.Action = "Delete";
                        break;
                    case (int)Constant.AuditLogAction.Login:
                        auditLog.Action = "Login";
                        break;
                    case (int)Constant.AuditLogAction.Logout:
                        auditLog.Action = "Logout";
                        break;
                }

                auditLogs.Add(auditLog);
            }

            return auditLogs;
        }

        public int GetAuditLogsCount(string filterText)
        {
            var parameters = new Dictionary<string, object>
            {
                { "searchText", "%" + filterText + "%"},
            };

            int count = Convert.ToInt32(_databaseAccess.GetSingleData(GetAuditLogsCountQuery, parameters));

            return count;
        }
    }
}
