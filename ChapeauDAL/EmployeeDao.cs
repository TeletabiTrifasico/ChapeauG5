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










        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            string query = "SELECT * FROM Employee";

            return (await ExecuteQueryAsync(query, reader => new Employee
            {
                EmployeeId = (int)reader["employee_id"],
                Username = reader["username"].ToString(),
                PasswordHash = reader["password"].ToString(),
                FirstName = reader["first_name"].ToString(),
                LastName = reader["last_name"].ToString(),
                PhoneNumber = reader["phone_number"].ToString(),
                Email = reader["email"].ToString(),
                IsActive = (bool)reader["is_active"],
                Role = Enum.TryParse<EmployeeRole>(reader["role"].ToString(), out var role) ? role : EmployeeRole.Waiter // or another sensible default
            })).ToList();
        }

        public void AddEmployee(Employee emp)
        {
            string query = @"
    INSERT INTO Employee (username, password, first_name, last_name, phone_number, role, email, is_active)
    VALUES (@username, @password, @first_name, @last_name, @phone, @role, @email, 1)";

            SqlParameter[] parameters = {
        CreateParameter("@username", emp.Username),
        CreateParameter("@password", emp.PasswordHash),
        CreateParameter("@first_name", emp.FirstName),
        CreateParameter("@last_name", emp.LastName),
        CreateParameter("@phone", emp.PhoneNumber),
        CreateParameter("@role", emp.Role.ToString()),  // FIXED
        CreateParameter("@email", emp.Email)
    };

            ExecuteEditQuery(query, parameters);
        }

        public void UpdateEmployee(Employee emp)
        {
            string query = @"
        UPDATE Employee
        SET 
            first_name = @first_name,
            last_name = @last_name,
            username = @username,
            email = @email,
            role = @role,
            is_active = @is_active
        WHERE employee_id = @employee_id";

            var parameters = new[]
            {
        new SqlParameter("@first_name", emp.FirstName),
        new SqlParameter("@last_name", emp.LastName),
        new SqlParameter("@username", emp.Username),
        new SqlParameter("@email", emp.Email),
        new SqlParameter("@role", emp.Role.ToString()), // ust match DB exactly
        new SqlParameter("@is_active", emp.IsActive),
        new SqlParameter("@employee_id", emp.EmployeeId)
    };

            ExecuteEditQuery(query, parameters);
        }

        public void SetEmployeeActiveStatus(int id, bool isActive)
        {
            string query = "UPDATE Employee SET is_active = @active WHERE employee_id = @id";
            ExecuteEditQuery(query, new[] {
            CreateParameter("@active", isActive),
            CreateParameter("@id", id)
        });
        }

        public void DeleteEmployee(int employeeId)
        {
            string query = "DELETE FROM Employee WHERE employee_id = @EmployeeId";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@EmployeeId", employeeId)
            };

            ExecuteEditQuery(query, parameters);
        }

    }
}