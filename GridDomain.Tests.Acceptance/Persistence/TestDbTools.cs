using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using GridDomain.Balance.ReadModel;
using GridDomain.EventStore.MSSQL.LogPersistance;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.Persistence
{
    public static class TestDbTools
    {

        public static void ClearAll(IDbConfiguration conf)
        {
            //for magic in clear db by delete
            BusinessBalanceContext.DefaultConnectionString = conf.LogsConnectionString;
            LogContext.DefaultConnectionString = conf.ReadModelConnectionString;

            using (var balanceContext = new BusinessBalanceContext(conf.ReadModelConnectionString))
                ClearDbByDelete(balanceContext);

            using (var logContext = new LogContext(conf.LogsConnectionString))
                ClearDbByDelete(logContext);

            RecreateWriteDb(conf.EventStoreConnectionString);
        }

        public static void ClearDbByDelete(DbContext context)
        {
            //TODO: понять, почему вызывает конструктор контекста по-умолчанию и как отучить 
            ObjectContext objectContext = null;
            try
            {
                context.Database.CreateIfNotExists();
                objectContext = ((IObjectContextAdapter) context).ObjectContext;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("DB clearing exception");//shut up with damn CreatedOn
            }
            var tableNames = objectContext.MetadataWorkspace
                .GetItemCollection(DataSpace.SSpace)
                .GetItems<EntityContainer>()
                .SelectMany(c => c.BaseEntitySets.Select(s => s.Table))
                .Where(t => t != null);

            foreach (var tableName in tableNames)
            {
                context.Database.ExecuteSqlCommand($"delete from {tableName}");
            }
        }

        public static void RecreateWriteDb(string eventStoreConnectionString)
        {
            using (var conn = new SqlConnection(eventStoreConnectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "TRUNCATE TABLE Commits";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Truncate table Snapshots";
                cmd.ExecuteNonQuery();
            }
        }
    }
}