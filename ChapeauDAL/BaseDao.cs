using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace ChapeauG5.ChapeauDAL
{
    public abstract class BaseDao
    {
        protected readonly string connectionString;

        protected BaseDao()
        {
            // Get connection string from App.config
            connectionString = ConfigurationManager.ConnectionStrings["ChapeauG5DB"].ConnectionString;
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

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

        protected async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

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

        // Transaction helper method
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
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Helper method to create parameters
        protected SqlParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        // Helper method to create parameters with specific SQL type
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
