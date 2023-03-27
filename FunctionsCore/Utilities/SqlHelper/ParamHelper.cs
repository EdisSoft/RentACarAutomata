using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace FunctionsCore.Utilities.SqlHelper
{
    public class ParamHelper
    {
        public SqlParameter CreateSympleListParameter<T>(string parameterName, string typeSchemaName, string typeName, List<T> list) where T : notnull
        {
            string schemaAndTypeName = GetDatabaseStandardName(typeSchemaName, typeName);
            DataTable dataTable = GetTableForSympleListParam<T>(typeName);
            foreach (var item in list)
            {
                dataTable.Rows.Add(item);
            }

            SqlParameter parameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = SqlDbType.Structured,
                Value = dataTable,
                TypeName = schemaAndTypeName
            };

            return parameter;
        }

        DataTable GetTableForSympleListParam<T>(string typeName)
        {
            DataTable dataTable = new DataTable(typeName);
            Type type = typeof(T);
            Type nonNullableType = GetNonNullableType(type);
            bool allowDBNull = AllowDBNull(type);
            dataTable.Columns.Add(new DataColumn { ColumnName = "item", DataType = nonNullableType, AllowDBNull = allowDBNull });
            return dataTable;
        }

        internal bool AllowDBNull(Type type)
        {
            Type t = GetNonNullableType(type);
            bool typeIsString = t == typeof(string);
            bool allowDBNull = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return allowDBNull || typeIsString;
        }

        internal Type GetNonNullableType(Type type) => Nullable.GetUnderlyingType(type) ?? type;

        string GetDatabaseStandardName(string dbSchemaName, string dbObjectName) => $"[{Trim(dbSchemaName)}].[{Trim(dbObjectName)}]";

        internal string Trim(string str) => str.Trim().TrimStart('[').TrimEnd(']');

        internal DataTable GetTableForStructuredParam(PropertyInfo[] propInfos)
        {
            DataTable dataTable = new DataTable();
            foreach (var propInfo in propInfos)
            {
                Type t = GetNonNullableType(propInfo.PropertyType);
                dataTable.Columns.Add(new DataColumn { ColumnName = propInfo.Name, DataType = t, AllowDBNull = AllowDBNull(propInfo.PropertyType) });
            }
            return dataTable;
        }

        public SqlParameter GetStructuredPar<T>(string paramName, string paramTypeSchemaName, string paramTypeName, List<T> paramValue) where T : class
        {

            if (string.IsNullOrWhiteSpace(paramName)) throw new Exception($"{nameof(paramName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(paramTypeSchemaName)) throw new Exception($"{nameof(paramTypeSchemaName)} megadása kötelező!");
            if (string.IsNullOrWhiteSpace(paramTypeName)) throw new Exception($"{nameof(paramTypeName)} megadása kötelező!");
            if (paramValue == null) throw new Exception($"{nameof(paramValue)} értéke nem lehet null!");

            string schemaAndTypeName = GetDatabaseStandardName(paramTypeSchemaName, paramTypeName);

            DataRow row;
            PropertyInfo[] propInfos = typeof(T).GetProperties();
            DataTable dataTable = GetTableForStructuredParam(propInfos);

            foreach (T item in paramValue)
            {
                row = dataTable.NewRow();
                foreach (PropertyInfo propInfo in propInfos)
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = paramName,
                SqlDbType = SqlDbType.Structured,
                Value = dataTable,
                TypeName = schemaAndTypeName
            };
            
            return parameter;
        }
    }
}