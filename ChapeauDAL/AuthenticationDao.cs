using ChapeauG5.ChapeauModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauDAL
{
    public class AuthenticationDao : BaseDao
    {
        public async Task<Employee> AuthenticateEmployeeAsync(string username, string passwordHash)
        {
            string query = @"
                SELECT e.*
                FROM EMPLOYEE e
                WHERE e.username = @Username 
                AND e.passwordHash = @PasswordHash 
                AND e.is_active = 1";

            var parameters = new[]
            {
                CreateParameter("@Username", username),
                CreateParameter("@PasswordHash", passwordHash)
            };

            return await ExecuteQuerySingleAsync(query, MapEmployee, parameters);
        }

        private Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employee_id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("passwordHash")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Role = Enum.Parse<EmployeeRole>(reader.GetString(reader.GetOrdinal("role"))),
                PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"))
            };
        }
    }
}