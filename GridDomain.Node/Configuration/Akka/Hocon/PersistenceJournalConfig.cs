namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class PersistenceJournalConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akka;
        private readonly IAkkaConfig _eventAdatpersConfig;

        public PersistenceJournalConfig(AkkaConfiguration akka, IAkkaConfig @eventAdatpersConfig)
        {
            _eventAdatpersConfig = eventAdatpersConfig;
            _akka = akka;
        }

        public string Build()
        {
            var persistenceJournalConfig = @"
            journal {
                    plugin = ""akka.persistence.journal.sql-server""
                    

                    sql-server {
                               class = """+typeof(SqlDomainJournal).AssemblyQualifiedShortName() + @"""
                               plugin-dispatcher = ""akka.actor.default-dispatcher""
                               connection-string =  """ + _akka.Persistence.JournalConnectionString + @"""
                               connection-timeout = 30s
                               schema-name = dbo
                               table-name = """ + _akka.Persistence.JournalTableName + @"""
                               auto-initialize = on
                               timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                               metadata-table-name = """ + _akka.Persistence.MetadataTableName + @"""
                               " + _eventAdatpersConfig.Build() + @"
                    }
            }
";
            return persistenceJournalConfig;
        }
    }
}