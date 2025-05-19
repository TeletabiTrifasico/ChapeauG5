using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class LoginCredentials
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Employee ID")]
        public string Username { get; set; }

        [Required(ErrorMessage = "PIN is required")]
        [Display(Name = "PIN Code")]
        [DataType(DataType.Password)]
        public string PIN { get; set; }

        public bool RememberMe { get; set; } = false;
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Employee Employee { get; set; }
        public string ErrorCode { get; set; } // For specific error handling
    }
}
