using System;

namespace StudentEMS.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int UserId { get; set; }
        public string UserRole { get; set; }
        public string Password { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
