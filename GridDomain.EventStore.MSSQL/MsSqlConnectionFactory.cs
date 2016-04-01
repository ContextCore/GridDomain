using System;
using System.Data;
using System.Data.SqlClient;
using NEventStore.Persistence.Sql;

namespace GridDomain.EventStore.MSSQL
{
    public class MsSqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public MsSqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Open()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        public Type GetDbProviderFactoryType()
        {
            return typeof (MsSqlConnectionFactory);
        }
    }
}