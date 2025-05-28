using System;
using System.Collections.Generic;
using System.Data;
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
        private SqlDataAdapter adapter;

        // Initializes the connection string from App.config
        protected BaseDao()
        {
            try
            {
                // Use ConfigurationManager to get connection string from app.config
                connectionString = ConfigurationManager.ConnectionStrings["ChapeauG5Db"].ConnectionString;
            }
            catch (ConfigurationErrorsException)
            {
                // Fallback connection string if config is missing
                connectionString = "Data Source=.;Initial Catalog=ChapeauG5Db;Integrated Security=True";
            }
            
            // Initialize adapter for sync methods
            adapter = new SqlDataAdapter();
        }

        // Creates a new SQL Server connection using the stored connection string
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        #region Async Methods

        // Executes a SQL query that returns a single row and maps it to an object
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

        #endregion

        #region Synchronous Methods

        /* For SELECT queries returning multiple records */
        protected DataTable ExecuteSelectQuery(string query, SqlParameter[] sqlParameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand command = new SqlCommand(query, conn);
                if (sqlParameters != null)
                    command.Parameters.AddRange(sqlParameters);
                
                DataTable dataTable = new DataTable();
                adapter.SelectCommand = command;
                
                conn.Open();
                adapter.Fill(dataTable);
                
                return dataTable;
            }
        }

        /* For INSERT queries returning the ID of the new record */
        protected int ExecuteInsertQuery(string query, SqlParameter[] sqlParameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand command = new SqlCommand(query, conn);
                if (sqlParameters != null)
                    command.Parameters.AddRange(sqlParameters);
                
                conn.Open();
                int id = Convert.ToInt32(command.ExecuteScalar());
                
                return id;
            }
        }

        /* For UPDATE and DELETE queries */
        protected void ExecuteEditQuery(string query, SqlParameter[] sqlParameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand command = new SqlCommand(query, conn);
                if (sqlParameters != null)
                    command.Parameters.AddRange(sqlParameters);
                
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        #endregion

        // Helper method to create SQL parameters with null handling
        protected SqlParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }
    }
}