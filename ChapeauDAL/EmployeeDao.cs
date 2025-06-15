using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChapeauModel;
using Microsoft.Data.SqlClient;

namespace ChapeauDAL
{
    public class EmployeeDao : BaseDao
    {
        // Get employee by username
        public async Task<Employee> GetByUsernameAsync(string username)
        {
            string query = "SELECT * FROM EMPLOYEE WHERE username = @Username";
            var parameters = new SqlParameter[] {
                new SqlParameter("@Username", username)
            };
            
            return await ExecuteQuerySingleAsync(query, ReadEmployee, parameters);
        }

        // Get employee by ID
        public async Task<Employee> GetByIdAsync(int id)
        {
            string query = "SELECT * FROM EMPLOYEE WHERE employee_id = @EmployeeId";
            var parameters = new SqlParameter[] {
                new SqlParameter("@EmployeeId", id)
            };
            
            return await ExecuteQuerySingleAsync(query, ReadEmployee, parameters);
        }

        // Helper method to read employee data from SqlDataReader
        private Employee ReadEmployee(SqlDataReader reader)
        {
            Employee employee = new Employee
            {
                EmployeeId = (int)reader["employee_id"],
                Username = (string)reader["username"],
                //PasswordHash = (string)reader["password"], // Changed from passwordHash to password as that is its name in the DB
                PasswordHash = (string)reader["PasswordHash"],
                FirstName = (string)reader["first_name"],
                LastName = (string)reader["last_name"],
                PhoneNumber = reader["phone_number"] as string,
                Role = ParseEmployeeRole((string)reader["role"]),
                Email = (string)reader["email"],
                IsActive = (bool)reader["is_active"]
            };
            
            return employee;
        }
        
        // Parse string role to enum
        private EmployeeRole ParseEmployeeRole(string role)
        {
            return Enum.Parse<EmployeeRole>(role);
        }
    }
}