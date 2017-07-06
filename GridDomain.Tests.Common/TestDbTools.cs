using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common
{
    public static class TestDbTools
    {
        public static async Task ClearData(IAkkaDbConfiguration akkaConf)
        {
            await Truncate(akkaConf.SnapshotConnectionString.Replace("\\\\", "\\"), akkaConf.SnapshotTableName);
            await Truncate(akkaConf.JournalConnectionString.Replace("\\\\", "\\"),
                         akkaConf.JournalTableName,
                         akkaConf.MetadataTableName);
        }

        public static async Task Truncate(string connection, params string[] tableNames)
        {
            await ExecuteSql(connection, tableNames.Select(t => $"IF OBJECT_ID('{t}') IS NOT NULL BEGIN  Truncate table {t} END ").ToArray());
        }

        private static async Task Delete(string connection, params string[] tableNames)
        {
            await ExecuteSql(connection, tableNames.Select(t => $"IF OBJECT_ID('{t}') IS NOT NULL BEGIN  Delete from {t}  END").ToArray());
        }

        private static async Task ExecuteSql(string connection, params string[] sqlCommand)
        {
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                foreach (var sqlrequest in sqlCommand)
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sqlrequest;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task RecreateWriteDb(string eventStoreConnectionString)
        {
            using (var conn = new SqlConnection(eventStoreConnectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "TRUNCATE TABLE Commits";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "Truncate table Snapshots";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}