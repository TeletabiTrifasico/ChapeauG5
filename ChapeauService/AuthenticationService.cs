using ChapeauDAL;
using ChapeauModel;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauService
{
    public class AuthenticationService
    {
        private EmployeeDao employeeDao;
        
        public AuthenticationService()
        {
            employeeDao = new EmployeeDao();
        }
        
        // Add synchronous version for compatibility with LoginForm
        public Employee Login(string username, string password)
        {
            return LoginAsync(username, password).GetAwaiter().GetResult();
        }
        
        // Change the method to be async and return a Task<Employee>
        public async Task<Employee> LoginAsync(string username, string password)
        {
            try
            {
                // Get employee by username - use the correct async method name
                Employee employee = await employeeDao.GetByUsernameAsync(username);
                
                // Check if employee exists and is active
                if (employee == null)
                {
                    throw new Exception("Username does not exist.");
                }
                
                if (!employee.IsActive)
                {
                    throw new Exception("This account is inactive. Please contact management.");
                }
                
                // Verify password
                if (!VerifyPassword(password, employee.PasswordHash))
                {
                    throw new Exception("Incorrect password.");
                }
                
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed: " + ex.Message);
            }
        }
        
        // Verify the password against the stored hash
        private bool VerifyPassword(string password, string storedHash)
        {
            // In a real application, use a proper password hashing library like BCrypt
            // This is a simple SHA256 implementation for demonstration
            string hashedInput = ComputeSha256Hash(password);
            return hashedInput == storedHash;
        }
        
        // Simple SHA256 hashing for demonstration
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                
                // Convert byte array to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert to lowercase hex
                }
                return builder.ToString();
            }
        }
    }
}