using System.Data.SqlClient;
using System.Linq;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;

namespace GridDomain.Tests.Framework
{
    public static class TestDbTools
    {
        public static void ClearData(IAkkaDbConfiguration akkaConf)
        {
            ClearWriteAkkaDb(akkaConf);
        }

        public static void ClearData(IDbConfiguration conf)
        {
            //Delete(conf.ReadModelConnectionString, "TransactionHistories");
            //Delete(conf.ReadModelConnectionString, "BusinessBalances");
        }

        public static void ClearData(IDbConfiguration conf, IAkkaDbConfiguration akkaConf)
        {
            ClearData(conf);
            ClearData(akkaConf);
        }

        private static void ClearWriteAkkaDb(IAkkaDbConfiguration akkaConf)
        {
            Truncate(akkaConf.SnapshotConnectionString.Replace("\\\\", "\\"), akkaConf.SnapshotTableName);
            Truncate(akkaConf.JournalConnectionString.Replace("\\\\", "\\"), akkaConf.JournalTableName,
                akkaConf.MetadataTableName);
        }

        private static void Truncate(string connection, params string[] tableNames)
        {
            ExecuteSql(connection, tableNames.Select(t => $"Truncate table {t}").ToArray());
        }

        private static void Delete(string connection, params string[] tableNames)
        {
            ExecuteSql(connection, tableNames.Select(t => $"Delete from {t}").ToArray());
        }

        private static void ExecuteSql(string connection, params string[] sqlCommand)
        {
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                foreach (var sqlrequest in sqlCommand)
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sqlrequest;
                    cmd.ExecuteNonQuery();
                }
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