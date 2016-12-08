using System;

namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class PersistenceJournalConfig : IAkkaConfig
    {
        private readonly IAkkaConfig _eventAdatpersConfig;
        private readonly IAkkaDbConfiguration _dbConfiguration;
        private readonly Type _sqlJournalType;


        public PersistenceJournalConfig(IAkkaDbConfiguration dbConfiguration, IAkkaConfig eventAdatpersConfig, Type sqlJournalType = null)
        {
            _eventAdatpersConfig = eventAdatpersConfig;
            _dbConfiguration = dbConfiguration;
            _sqlJournalType = sqlJournalType ?? typeof(SqlDomainJournal);
        }

        public string Build()
        {
            var jornalConnectionTimeoutSeconds = _dbConfiguration.JornalConnectionTimeoutSeconds;
            if (jornalConnectionTimeoutSeconds <= 0) jornalConnectionTimeoutSeconds = 30;

            var persistenceJournalConfig = @"
            journal {
                    plugin = ""akka.persistence.journal.sql-server""

                    sql-server {
                               class = """+_sqlJournalType.AssemblyQualifiedShortName() + @"""
                               plugin-dispatcher = ""akka.actor.default-dispatcher""
                               connection-string =  """ + _dbConfiguration.JournalConnectionString + @"""
                               connection-timeout = "+jornalConnectionTimeoutSeconds+@"s
                               schema-name = dbo
                               table-name = """ + _dbConfiguration.JournalTableName + @"""
                               auto-initialize = on
                               timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                               metadata-table-name = """ + _dbConfiguration.MetadataTableName + @"""
                               " + _eventAdatpersConfig.Build() + @"
                    }
            }
";
            return persistenceJournalConfig;
        }
    }
}