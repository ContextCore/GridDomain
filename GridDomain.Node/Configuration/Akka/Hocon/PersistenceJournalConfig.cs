namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class PersistenceJournalConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akka;

        public PersistenceJournalConfig(AkkaConfiguration akka)
        {
            _akka = akka;
        }

        public string Build()
        {
            return BuildPersistenceJournalConfig(_akka);
        }

        public static string BuildPersistenceJournalConfig(AkkaConfiguration akkaConf)
        {
            var persistenceJournalConfig = @"
            journal {
                    plugin = ""akka.persistence.journal.sql-server""
                    event-adapters
                    {
                        domainEventsUpgrade = ""GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.BalanceChangedEventAdapter, GridDmoin.Tests.Acceptance""
                    }
                   
                    event-adapter-bindings
                    {
                        ""GridDomain.EventSourcing.DomainEvent, GridDomain.EventSourcing"" = domainEventsUpgrade
                    }

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
    }
}