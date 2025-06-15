using System;

namespace ChapeauModel
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public EmployeeRole Role { get; set; }
        public required string Email { get; set; }
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