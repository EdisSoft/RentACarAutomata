using FunctionsCore.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FunctionsCore.Utilities
{
    public static class DbExtensions2
    {
        public static List<T> SafeExecuteReader<T>(this DbContext context, string query)
        {
            return SafeExecuteReader<T>(context, query, null);
        }
        public static List<T> SafeExecuteReader<T>(this DbContext context, string query, int maxEnabledJsonErrorCount = 0, int maxTryCountIfWasDbDeadlock = 1)
        {
            return SafeExecuteReader<T>(context, query, maxEnabledJsonErrorCount, maxTryCountIfWasDbDeadlock, null);
        }

        public static List<T> SafeExecuteReader<T>(this DbContext context, string query, params SqlParameter[] sqlParams)
        {
            return SafeExecuteReader<T>(context, query, 0, 1, sqlParams);
        }

        public static List<T> SafeExecuteReader<T>(this DbContext context, string query, int maxEnabledJsonErrorCount = 0, int maxTryCountIfWasDbDeadlock = 1, params SqlParameter[] sqlParams)
        {
            var retryCount = 0;
            List<T> resultList = new List<T>();
            while (retryCount < maxTryCountIfWasDbDeadlock)
            {
                var conn = context.Database.GetDbConnection();
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandTimeout = context.Database.GetCommandTimeout() ?? command.CommandTimeout;
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        if (sqlParams != null) command.Parameters.AddRange(sqlParams);
                        if (context.Database.CurrentTransaction == null)
                        {
                            if (conn.State != ConnectionState.Open)
                                conn.Open();
                        }
                        else
                        {
                            command.Transaction = context.Database.CurrentTransaction.GetDbTransaction();
                        }
                        using (var result = command.ExecuteReader())
                        {
                            var mapper = new DbDataReaderMapper<T>(result, maxEnabledJsonErrorCount);
                            resultList = mapper.MapData();
                            return resultList;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)// Deadlock
                    {
                        retryCount++;
                        if (retryCount < maxTryCountIfWasDbDeadlock)
                        {
                            Log.Warning($"Deadlock történt {retryCount}. futásra, újra próbálkozás...", ex);
                        }
                        else
                            throw;
                    }
                    else
                        throw;
                }
                finally
                {
                    if (context.Database.CurrentTransaction == null)
                        conn.Close();
                }
            }
            return resultList;
        }

        public class DbDataReaderMapper<T>
        {
            public DbDataReader Reader { get; set; }

            public List<ColumnData> Columns { get; set; }

            public List<T> Data { get; set; }

            public int MaxEnabledJsonErrorCount { get; set; }

            public List<MappingError> Errors { get; set; }
            public DbDataReaderMapper(DbDataReader reader, int maxEnabledJsonErrorCount)
            {
                Reader = reader;
                MaxEnabledJsonErrorCount = maxEnabledJsonErrorCount;
                Errors = new List<MappingError>();

            }

            public List<T> MapData()
            {
                MapColumns(Reader);
                MapData(Reader);
                return Data;
            }

            public void MapColumns(DbDataReader reader)
            {
                var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).Select(x => new ColumnData(x)).ToList();

                var props = typeof(T).GetProperties().Where(x => x.CanWrite).Select(x => new PropertyData(x)).ToDictionary(x => x.CompareName);

                foreach (var item in columnNames.ToList())
                {
                    if (props.ContainsKey(item.CompareName))
                    {
                        item.PropData = props[item.CompareName];
                    }
                }

                Columns = columnNames;
            }

            public void MapData(DbDataReader reader)
            {
                int i = 0;
                var cols = Columns.ToArray();
                object val = null;
                object propVal;
                Data = new List<T>();

                var jsonErrorHandler = new JsonSerializerSettings
                {
                    Error = delegate (object sender, ErrorEventArgs args)
                    {
                        var error = new MappingError()
                        {
                            ErrorColumnName = cols[i].Data,
                            ErrorColumnValue = val as string,
                            ErrorColumnType = cols[i].PropData.Data.PropertyType.FullName,
                            ReturnType = typeof(T).FullName,
                            Exception = args.ErrorContext.Error
                        };
                        var idColumn = cols.SingleOrDefault(x => x.CompareName == "id");
                        if (idColumn != null)
                        {
                            error.RowId = reader[idColumn.Data].ToString();
                        }
                        Errors.Add(error);
                        Log.Error($"JSON parse hiba\r\n" +
                                  $"List type: {error.ReturnType}\r\n" +
                                  $"RowId: {error.RowId}\r\n" +
                                  $"Property type: {error.ErrorColumnType}\r\n" +
                                  $"Column:{error.ErrorColumnName}\r\n" +
                                  $"Value:{error.ErrorColumnValue}\r\n" +
                                  $"{Environment.StackTrace}\r\n"
                                  , error.Exception);

                        args.ErrorContext.Handled = true;

                    },
                };

                var notMappedProperties = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(NotMappedAttribute))).ToList();
                while (reader.Read())
                {
                    var element = Activator.CreateInstance<T>();
                    for (i = 0; i < cols.Length; i++)
                    {
                        if (notMappedProperties.Any(c => c.Name == cols[i].Data))
                            continue;
                        val = reader.GetValue(i);
                        if (cols[i].PropData != null)
                        {
                            if (val == null || val == DBNull.Value)
                            {
                                val = null;
                            }
                            else
                            {
                                if (cols[i].PropData.IsJson)
                                {
                                    propVal = JsonConvert.DeserializeObject(val as string, cols[i].PropData.Data.PropertyType, jsonErrorHandler);
                                }
                                else
                                {
                                    propVal = Convert.ChangeType(val, Nullable.GetUnderlyingType(cols[i].PropData.Data.PropertyType) ?? cols[i].PropData.Data.PropertyType);
                                }
                                cols[i].PropData.Data.SetValue(element, propVal);
                            }
                        }
                    }
                    Data.Add(element);
                    if (Errors.Count > MaxEnabledJsonErrorCount)
                    {
                        throw new Exception("Hiba a lekérdezés beolvasása közben");
                    }
                }

            }

        }

        public class MappingError
        {
            public string RowId { get; set; }
            public string ErrorColumnName { get; set; }
            public string ErrorColumnValue { get; set; }
            public string ReturnType { get; set; }
            public string ErrorColumnType { get; set; }
            public Exception Exception { get; set; }

        }

        public class PropertyData
        {
            public PropertyInfo Data { get; set; }

            public string CompareName { get; set; }

            public bool IsJson { get; set; }

            public PropertyData(PropertyInfo data)
            {
                Data = data;
                CompareName = data.Name.ToLower();
                CheckJsonType();
            }

            void CheckJsonType()
            {
                IsJson = Data.PropertyType.GetCustomAttributes(typeof(JsonEntityAttribute), true).Length > 0;
            }
        }

        public class ColumnData
        {
            public string Data { get; set; }

            public string CompareName { get; set; }

            public PropertyData PropData { get; set; }

            public ColumnData(string data)
            {
                Data = data;
                CompareName = data.ToLower();
            }
        }

    }
}
