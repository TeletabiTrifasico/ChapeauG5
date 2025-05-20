using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace ChapeauDAL
{
    // All data access objects (DAOs) should inherit from this class.
    public abstract class BaseDao
    {
        // The database connection string loaded from App.config
        protected readonly string connectionString;

        // Initializes the connection string from App.config
        protected BaseDao()
        {
            // Get connection string from App.config using the "ChapeauG5DB" key this has to be the same as in the App.config file
            connectionString = ConfigurationManager.ConnectionStrings["ChapeauG5DB"].ConnectionString;
        }

        // Creates a new SQL Server connection using the stored connection string
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Executes a SQL query that returns a single value (like COUNT, SUM, etc.)
        protected async Task<T> ExecuteScalarAsync<T>(string query, params SqlParameter[] parameters)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != DBNull.Value ? (T)result : default(T);
        }

        // Executes a SQL query that doesn't return data (INSERT, UPDATE, DELETE)
        protected async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        // Executes a SQL query that returns multiple rows and maps them to objects
        protected async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, Func<SqlDataReader, T> mapper, params SqlParameter[] parameters)
        {
            var results = new List<T>();

            using var connection = GetConnection();
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(mapper(reader));
            }

            return results;
        }

        // Executes a SQL query that returns a single row and maps it to an object
        // Useful for queries like "SELECT * FROM table WHERE id = @id"
        protected async Task<T> ExecuteQuerySingleAsync<T>(string query, Func<SqlDataReader, T> mapper, params SqlParameter[] parameters)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return mapper(reader);
            }

            return default(T);
        }

        // Executes a SQL query and returns the results as a DataTable
        // Useful for scenarios where you need tabular data
        protected async Task<DataTable> ExecuteQueryToDataTableAsync(string query, params SqlParameter[] parameters)
        {
            var dataTable = new DataTable();

            using var connection = GetConnection();
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            dataTable.Load(reader);

            return dataTable;
        }

        // Executes multiple SQL commands within a single transaction
        // If any operation fails, the entire transaction is rolled back
        protected async Task<bool> ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task<bool>> operations)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var success = await operations(connection, transaction);

                if (success)
                {
                    // If operations returned true, commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    // If operations returned false, roll back the transaction
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                // If any exception occurs, roll back the transaction
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Creates a SQL parameter with a name and value
        // Handles null values by converting them to DBNull.Value
        protected SqlParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        // Creates a SQL parameter with a name, value, and specific SQL data type
        // Use this when you need to control the exact SQL type (date/time or decimal values)
        protected SqlParameter CreateParameter(string name, object value, SqlDbType sqlDbType)
        {
            var parameter = new SqlParameter(name, sqlDbType)
            {
                Value = value ?? DBNull.Value
            };
            return parameter;
        }
    }
}