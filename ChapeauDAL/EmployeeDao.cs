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
            // ExecuteQuerySingleAsync is a method in BaseDao that executes a query and maps the result to an Employee object
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

        //This is mapping. It takes a SqlDataReader and maps it to an Employee object.
        //mapping = reading data from the database and converting it to an object
        private Employee ReadEmployee(SqlDataReader reader)
        {
            Employee employee = new Employee
            {
                EmployeeId = (int)reader["employee_id"],
                Username = (string)reader["username"],
                PasswordHash = (string)reader["password"], // Changed from passwordHash to password as that is its name in the DB
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