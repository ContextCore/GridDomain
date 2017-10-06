using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Persistence.Sql
{
    internal class PersistenceSnapshotConfig : IHoconConfig
    {
        private readonly ISqlNodeDbConfiguration _sqlNodeDbConfiguration;

        public PersistenceSnapshotConfig(ISqlNodeDbConfiguration sqlNodeDbConfiguration)
        {
            _sqlNodeDbConfiguration = sqlNodeDbConfiguration;
        }

        public string Build()
        {
            return BuildPersistenceSnapshotConfig(_sqlNodeDbConfiguration);
        }

        public static string BuildPersistenceSnapshotConfig(ISqlNodeDbConfiguration sqlNodeDbConfiguration)
        {
            var persistenceSnapshotStorageConfig = @" 
            snapshot-store {
                           plugin =  ""akka.persistence.snapshot-store.sql-server""
                           sql-server {
                                      class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                      plugin-dispatcher = ""akka.actor.default-dispatcher""
                                      connection-string = """ + sqlNodeDbConfiguration.SnapshotConnectionString + @"""
                                      connection-timeout = " + sqlNodeDbConfiguration.SnapshotsConnectionTimeoutSeconds + @"s
                                      schema-name = "+ sqlNodeDbConfiguration.SchemaName + @"
                                      table-name = """ + sqlNodeDbConfiguration.SnapshotTableName + @"""
                                      auto-initialize = on
                           }
            }";
            return persistenceSnapshotStorageConfig;
        }
    }
}