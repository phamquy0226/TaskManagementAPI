using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace QuanLyCongViecAPI.Helpers
{
    public class DatabaseHelper
    {
        private readonly IConfiguration _configuration;

        public DatabaseHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("WorkManagementDB_EN");
        }

        public Tuple<bool, string, DataTable> ExecuteStoredProcedureWithStatus(string storedProcedureName, SqlParameter[] parameters = null)
        {
            string connectionString = GetConnectionString();
            bool success = false;
            string errorMessage = null;
            DataTable data = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            success = Convert.ToBoolean(reader["Success"]);
                            errorMessage = reader["ErrorMessage"]?.ToString();
                        }

                        if (success && reader.NextResult())
                        {
                            data = new DataTable();
                            data.Load(reader);
                        }
                    }
                }
            }

            return Tuple.Create(success, errorMessage, data);
        }


        public Tuple<bool, string, int?> ExecuteNonQueryStoredProcedureWithStatus(string storedProcedureName, SqlParameter[] parameters = null)
        {
            string connectionString = GetConnectionString();
            bool success = false;
            string errorMessage = null;
            int? generatedId = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    SqlParameter outputIdParam = parameters?.FirstOrDefault(p => p.Direction == ParameterDirection.Output);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    success = true;

                    if (outputIdParam != null && outputIdParam.Value != DBNull.Value)
                    {
                        generatedId = Convert.ToInt32(outputIdParam.Value);
                    }
                }
            }

            return Tuple.Create(success, errorMessage, generatedId);
        }

        public List<T> MapDataTableToList<T>(DataTable dataTable) where T : new()
        {
            List<T> list = new List<T>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    T obj = new T();
                    foreach (PropertyInfo prop in typeof(T).GetProperties())
                    {
                        if (dataTable.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                        {
                            Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            object safeValue = Convert.ChangeType(row[prop.Name], targetType);
                            prop.SetValue(obj, safeValue);
                        }
                    }
                    list.Add(obj);
                }
            }
            return list;
        }
    }
}
