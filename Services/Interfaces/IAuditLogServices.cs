using StudentEMS.Models;
using System.Collections.Generic;

namespace StudentEMS.Services.Interfaces
{
    public interface IAuditLogServices
    {
        void GenerateAuditLog(int actionType, string userName, string actionDescription);
        string GetFullName(string userName);
        List<AuditLog> GetAuditLogsInfo(int limit, int offset, string filterText);
        int GetAuditLogsCount(string filterText);
    }
}
