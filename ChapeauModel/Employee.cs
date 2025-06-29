using System;

namespace ChapeauModel
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public EmployeeRole Role { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public enum EmployeeRole
    {
        Waiter,
        Bar,
        Kitchen,
        Manager
    }
}