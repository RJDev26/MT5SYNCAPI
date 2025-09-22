namespace OTS.Infrastructutre.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    namespace WebBroker.DataAccessCore
    {
        public class DbManager
        {
            private readonly string _connectionString;
            private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyCache
       = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

            private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache;

            private List<DbParameter> _currentParams;



            public List<DbParameter> OutParameters { get; private set; } = new();

            public DbManager(string connectionString)
            {
                _connectionString = connectionString;
            }

            private async Task<SqlConnection> OpenConnectionAsync()
            {
                var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                return connection;
            }

            private Dictionary<string, PropertyInfo> GetPropertyMap<T>()
            {
                var type = typeof(T);
                if (!PropertyCache.TryGetValue(type, out var map))
                {
                    map = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (prop.CanWrite)
                            map[prop.Name] = prop;
                    }
                    PropertyCache[type] = map;
                }
                return map;
            }

            private void UpdateOutParameters(SqlCommand command)
            {
                OutParameters = new List<DbParameter>();
                foreach (SqlParameter param in command.Parameters)
                {
                    if (param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput)
                    {
                        OutParameters.Add(new DbParameter(param.ParameterName.TrimStart('@'), param.Direction, param.Value));
                    }
                }

                // Update passed-in parameters too (important!)
                foreach (SqlParameter param in command.Parameters)
                {
                    var match = _currentParams?.FirstOrDefault(p => p.Name.Equals(param.ParameterName.TrimStart('@'), StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        match.Value = param.Value;
                    }
                }
            }
           

            private void AddParameters(SqlCommand command, List<DbParameter> parameters)
            {
                _currentParams = parameters;
                if (parameters == null) return;
                command.Parameters.Clear();

                foreach (var p in parameters)
                {
                    var sqlParam = new SqlParameter
                    {
                        ParameterName = "@" + p.Name,
                        Direction = p.Direction,
                        Value = p.Value ?? DBNull.Value
                    };

                    // ✅ Set Size if applicable (only for output or inputoutput string parameters)
                    if ((sqlParam.Direction == ParameterDirection.Output || sqlParam.Direction == ParameterDirection.InputOutput) &&
                        (p.Value == null || p.Value is string))
                    {
                        sqlParam.Size = p.Size ?? 200; // fallback to 200 if not specified
                    }

                    command.Parameters.Add(sqlParam);
                }
            }



            public async Task<List<T>> ExecuteListAsync<T>(string procedureName, List<DbParameter> parameters) where T : new()
            {
                var resultList = new List<T>();
                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(command, parameters);

                var propertyMap = GetPropertyMap<T>();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var obj = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        if (propertyMap.TryGetValue(columnName, out var prop))
                        {
                            var value = reader.GetValue(i);
                            if (value != DBNull.Value)
                            {
                                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                prop.SetValue(obj, Convert.ChangeType(value, targetType));
                            }
                        }
                    }
                    resultList.Add(obj);
                }

                UpdateOutParameters(command);
                return resultList;
            }

            public async Task<string> ExecuteScalarAsync(string procedureName, List<DbParameter> parameters)
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(command, parameters);
                var result = await command.ExecuteScalarAsync();
                UpdateOutParameters(command);
                return Convert.ToString(result);
            }

            public async Task<int> ExecuteNonQueryAsync(string procedureName, List<DbParameter> parameters)
            {
                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(command, parameters);
                var result = await command.ExecuteNonQueryAsync();
                UpdateOutParameters(command);
                return result;
            }

            public async Task<DataTable> ExecuteDataTableAsync(string procedureName, List<DbParameter> parameters)
            {
                var dataTable = new DataTable();
                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(command, parameters);
                await using var reader = await command.ExecuteReaderAsync();
                dataTable.Load(reader);
                UpdateOutParameters(command);
                return dataTable;
            }
            public async Task<(List<T1> List1, T2 Single2)> ExecuteMultipleAsync<T1, T2>(string procedureName, List<DbParameter> parameters)
    where T1 : new()
    where T2 : new()
            {
                var list1 = new List<T1>();
                T2 single2 = default;

                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(command, parameters);
                var propertyMap1 = GetPropertyMap<T1>();
                var propertyMap2 = GetPropertyMap<T2>();

                await using var reader = await command.ExecuteReaderAsync();

                // First result set: list of T1
                while (await reader.ReadAsync())
                {
                    var obj = new T1();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        if (propertyMap1.TryGetValue(columnName, out var prop))
                        {
                            var value = reader.GetValue(i);
                            if (value != DBNull.Value)
                            {
                                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                prop.SetValue(obj, Convert.ChangeType(value, targetType));
                            }
                        }
                    }
                    list1.Add(obj);
                }

                // Second result set: single T2 row
                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        single2 = new T2();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            if (propertyMap2.TryGetValue(columnName, out var prop))
                            {
                                var value = reader.GetValue(i);
                                if (value != DBNull.Value)
                                {
                                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    prop.SetValue(single2, Convert.ChangeType(value, targetType));
                                }
                            }
                        }
                    }
                }

                UpdateOutParameters(command);
                return (list1, single2);
            }

            public async Task<(List<T1> List1, List<T2> List2)> ExecuteMultipleListAsync<T1, T2>(string procedureName, List<DbParameter> parameters)
                where T1 : new()
                where T2 : new()
            {
                var list1 = new List<T1>();
                var list2 = new List<T2>();

                await using var connection = await OpenConnectionAsync();
                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(command, parameters);

                var propertyMap1 = GetPropertyMap<T1>();
                var propertyMap2 = GetPropertyMap<T2>();

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var obj = new T1();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        if (propertyMap1.TryGetValue(columnName, out var prop))
                        {
                            var value = reader.GetValue(i);
                            if (value != DBNull.Value)
                            {
                                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                prop.SetValue(obj, Convert.ChangeType(value, targetType));
                            }
                        }
                    }
                    list1.Add(obj);
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var obj = new T2();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            if (propertyMap2.TryGetValue(columnName, out var prop))
                            {
                                var value = reader.GetValue(i);
                                if (value != DBNull.Value)
                                {
                                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    prop.SetValue(obj, Convert.ChangeType(value, targetType));
                                }
                            }
                        }
                        list2.Add(obj);
                    }
                }

                UpdateOutParameters(command);
                return (list1, list2);
            }

        }

        public enum ExecuteType
        {
            ExecuteReader,
            ExecuteNonQuery,
            ExecuteScalar
        }

        public class DbParameter
        {
            public string Name { get; set; }
            public ParameterDirection Direction { get; set; }
            public object Value { get; set; }
            public int? Size { get; set; } // ✅ Add this

            public DbParameter(string name, ParameterDirection direction, object value, int? size = null)
            {
                Name = name;
                Direction = direction;
                Value = value;
                Size = size;
            }
        }

    }
}
