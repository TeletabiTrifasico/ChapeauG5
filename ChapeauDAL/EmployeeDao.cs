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



        //Boozie Stuff
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new List<Employee>();
            string query = "SELECT * FROM Employee";

            using (SqlConnection conn = GetConnection()) // Ensure 'conn' is declared here
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn)) // 'conn' is now in scope
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            EmployeeId = (int)reader["employee_id"],
                            Username = reader["username"].ToString()!,
                            PasswordHash = reader["password"].ToString()!,
                            FirstName = reader["first_name"].ToString()!,
                            LastName = reader["last_name"].ToString()!,
                            PhoneNumber = reader["phone_number"] as string,
                            Role = ParseEmployeeRole(reader["role"].ToString()!),
                            Email = reader["email"].ToString()!,
                            IsActive = (bool)reader["is_active"],
                        });
                    }
                }
            }
            return employees;
        }

        public void AddEmployee(Employee employee)
        {
            string query = "INSERT INTO Employee (first_name, last_name, username, password, role, is_active) VALUES (@first_name, @last_name, @username, @password, @role, 1)";
            using (SqlConnection conn = GetConnection()) // Ensure 'conn' is declared here
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn)) // 'conn' is now in scope
                {
                    cmd.Parameters.AddWithValue("@first_name", employee.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", employee.LastName);
                    cmd.Parameters.AddWithValue("@username", employee.Username);
                    cmd.Parameters.AddWithValue("@password", employee.PasswordHash);
                    cmd.Parameters.AddWithValue("@role", employee.Role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            string query = "UPDATE Employee SET first_name = @first_name, last_name = @last_name, username = @username, password = @password, role = @role WHERE employee_id = @employee_id";
            using (SqlConnection conn = GetConnection()) // Added missing declaration for 'conn'
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", employee.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", employee.LastName);
                    cmd.Parameters.AddWithValue("@username", employee.Username);
                    cmd.Parameters.AddWithValue("@password", employee.PasswordHash);
                    cmd.Parameters.AddWithValue("@role", employee.Role);
                    cmd.Parameters.AddWithValue("@employee_id", employee.EmployeeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SetEmployeeActiveStatus(int employeeId, bool isActive)
        {
            string query = "UPDATE Employee SET is_active = @is_active WHERE employee_id = @employee_id";
            using (SqlConnection conn = GetConnection()) // Ensure 'conn' is declared here
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn)) // 'conn' is now in scope
                {
                    cmd.Parameters.AddWithValue("@is_active", isActive);
                    cmd.Parameters.AddWithValue("@employee_id", employeeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }






        // Parse string role to enum
        private EmployeeRole ParseEmployeeRole(string role)
        {
            return Enum.Parse<EmployeeRole>(role);
        }
    }
}