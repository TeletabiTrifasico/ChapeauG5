using ChapeauG5.ChapeauDAL;
using ChapeauG5.ChapeauModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauService
{
    public class AuthenticationService
    {
        private AuthenticationDao authDao;
        private static Employee ?_currentUser;

        public AuthenticationService()
        {
            authDao = new AuthenticationDao();
        }

        public async Task<LoginResult> LoginAsync(LoginCredentials credentials)
        {
            try
            {
                // Validate credentials
                if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.PIN))
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Please enter both username and PIN.",
                        ErrorCode = "MISSING_CREDENTIALS"
                    };
                }

                // Hash the PIN for comparison
                string hashedPIN = HashPIN(credentials.PIN);

                // Attempt to authenticate
                var employee = await authDao.AuthenticateEmployeeAsync(credentials.Username, hashedPIN);

                if (employee == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Invalid username or PIN.",
                        ErrorCode = "INVALID_CREDENTIALS"
                    };
                }

                if (!employee.IsActive)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Your account has been deactivated. Please contact management.",
                        ErrorCode = "INACTIVE_ACCOUNT"
                    };
                }

                // Successful login
                return new LoginResult
                {
                    Success = true,
                    Message = "Login successful",
                    Employee = employee
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "An error occurred during login. Please try again.",
                    ErrorCode = "LOGIN_ERROR"
                };
            }
        }

        public string HashPIN(string pin)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pin));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static Employee GetCurrentUser()
        {
            return _currentUser;
        }

        public static void SetCurrentUser(Employee employee)
        {
            _currentUser = employee;
        }

        public static void ClearCurrentUser()
        {
            _currentUser = null;
        }
    }
}