using System;

namespace StudentEMS.Models
{
    public class AuditLog : User
    {
        public string UserType { get; set; }
        public  string Action { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
