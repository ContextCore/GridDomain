using GridDomain.Node.Configuration;

internal class PersistenceSnapshotConfig:IAkkaConfig
{
    private AkkaConfiguration _akka;

    public PersistenceSnapshotConfig(AkkaConfiguration akka)
    {
        _akka = akka;
    }

    public static string BuildPersistenceSnapshotConfig(AkkaConfiguration akkaConf)
    {
        string persistenceSnapshotStorageConfig = @" 
            snapshot-store {
                           plugin =  ""akka.persistence.snapshot-store.sql-server""
                           sql-server {
                                      class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                      plugin-dispatcher = ""akka.actor.default-dispatcher""
                                      connection-string = """ + akkaConf.Persistence.SnapshotConnectionString + @"""
                                      connection-timeout = 30s
                                      schema-name = dbo
                                      table-name = """ + akkaConf.Persistence.SnapshotTableName + @"""
                                      auto-initialize = on
                           }
            }";
        return persistenceSnapshotStorageConfig;
    }

    public string Build()
    {
        return BuildPersistenceSnapshotConfig(_akka);
    }
}