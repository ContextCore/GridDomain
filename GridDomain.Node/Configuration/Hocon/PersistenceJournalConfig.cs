using GridDomain.Node.Configuration;

internal class PersistenceJournalConfig:IAkkaConfig
{
    private AkkaConfiguration _akka;

    public PersistenceJournalConfig(AkkaConfiguration akka)
    {
        _akka = akka;
    }

    public static string BuildPersistenceJournalConfig(AkkaConfiguration akkaConf)
    {
        string persistenceJournalConfig = @"
            journal {
                    plugin = ""akka.persistence.journal.sql-server""
                    sql-server {
                               class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                               plugin-dispatcher = ""akka.actor.default-dispatcher""
                               connection-string =  """ + akkaConf.Persistence.JournalConnectionString + @"""
                               connection-timeout = 30s
                               schema-name = dbo
                               table-name = """ + akkaConf.Persistence.JournalTableName + @"""
                               auto-initialize = on
                               timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                               metadata-table-name = """ + akkaConf.Persistence.MetadataTableName + @"""
                    }
            }
";
        return persistenceJournalConfig;
    }

    public string Build()
    {
        return BuildPersistenceJournalConfig(_akka);
    }
}