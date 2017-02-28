using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Shop.Infrastructure
{
    public class SqlSequenceProvider : ISequenceProvider,
                                       IDisposable
    {
        private const string DefaultCollectionName = "global";
        private readonly string _connectionString;
        private readonly IDictionary<string, Func<long>> _sequences = new Dictionary<string, Func<long>>();
        private SqlConnection _connection;
        private readonly object _lock = new object();

        public SqlSequenceProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public long GetNext(string sequenceName = null)
        {
            lock (_lock)
            {
                Func<long> sequenceGenerator;
                sequenceName = sequenceName ?? DefaultCollectionName;
                if (!_sequences.TryGetValue(sequenceName, out sequenceGenerator))
                {
                    CreateNewSequence(sequenceName);
                    sequenceGenerator = _sequences[sequenceName] = () => GetNextFromSqlSequence(sequenceName);
                }

                return sequenceGenerator();
            }
        }

        public void Connect()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        private void CreateNewSequence(string sequenceName)
        {
            var cmd = new SqlCommand(@"
                                          IF OBJECT_ID('" + sequenceName + @"') IS NULL
                                          CREATE SEQUENCE " + sequenceName + @"
                                              START WITH 1  
                                              INCREMENT BY 1
                                          ", _connection);
            cmd.ExecuteNonQuery();
        }

        private long GetNextFromSqlSequence(string sequenceName)
        {
            var cmd = new SqlCommand(@"SELECT NEXT VALUE FOR " + sequenceName, _connection);
            return (long) cmd.ExecuteScalar();
        }
    }
}