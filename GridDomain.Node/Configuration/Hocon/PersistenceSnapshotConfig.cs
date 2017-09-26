namespace GridDomain.Node.Configuration.Hocon
{
    internal class PersistenceSnapshotConfig : INodeConfig
    {
        private readonly NodeConfiguration _node;

        public PersistenceSnapshotConfig(NodeConfiguration node)
        {
            _node = node;
        }

        public string Build()
        {
            return BuildPersistenceSnapshotConfig(_node);
        }

        public static string BuildPersistenceSnapshotConfig(NodeConfiguration nodeConf)
        {
            var persistenceSnapshotStorageConfig = @" 
            snapshot-store {
                           plugin =  ""akka.persistence.snapshot-store.sql-server""
                           sql-server {
                                      class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                      plugin-dispatcher = ""akka.actor.default-dispatcher""
                                      connection-string = """ + nodeConf.Persistence.SnapshotConnectionString + @"""
                                      connection-timeout = " + nodeConf.Persistence.SnapshotsConnectionTimeoutSeconds + @"s
                                      schema-name = "+ nodeConf.Persistence.SchemaName + @"
                                      table-name = """ + nodeConf.Persistence.SnapshotTableName + @"""
                                      auto-initialize = on
                           }
            }";
            return persistenceSnapshotStorageConfig;
        }
    }
}