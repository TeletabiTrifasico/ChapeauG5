using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public EmployeeRole Role { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}";


    }
}
