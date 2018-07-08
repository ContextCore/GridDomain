using System;
using Akka.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Persistence.Sql
{
    internal class PersistenceJournalConfig : IHoconConfig
    {
        private readonly ISqlNodeDbConfiguration _dbConfiguration;
        private readonly IHoconConfig _eventAdatpersConfig;
        private readonly Type _sqlJournalType;
        private string _assemblyQualifiedShortName;

        public PersistenceJournalConfig(ISqlNodeDbConfiguration dbConfiguration,
                                        IHoconConfig eventAdatpersConfig,
                                        Type sqlJournalType = null)
        {
            _eventAdatpersConfig = eventAdatpersConfig;
            _dbConfiguration = dbConfiguration;
            _sqlJournalType = sqlJournalType;
        }

        public string Build()
        {
            var jornalConnectionTimeoutSeconds = _dbConfiguration.JornalConnectionTimeoutSeconds;
            if (jornalConnectionTimeoutSeconds <= 0)
                jornalConnectionTimeoutSeconds = 30;

            _assemblyQualifiedShortName = _sqlJournalType?.AssemblyQualifiedShortName()
                ?? "Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer";

            var persistenceJournalConfig = @"
            akka.persistence.journal {
                    plugin = ""akka.persistence.journal.sql-server""

                    sql-server {
                               class = """ + _assemblyQualifiedShortName + @"""
                               plugin-dispatcher = ""akka.actor.default-dispatcher""
                               connection-string =  """ + _dbConfiguration.JournalConnectionString + @"""
                               connection-timeout = " + jornalConnectionTimeoutSeconds + @"s
                               schema-name = "+_dbConfiguration.SchemaName + @"
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