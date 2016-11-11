using Akka.Configuration;
using Akka.Persistence.Sql.Common.Journal;
using Akka.Persistence.SqlServer.Journal;

namespace GridDomain.Node
{
    public class SqlDomainJournal : SqlServerJournal
    {
        public SqlDomainJournal(Config config) : base(config)
        {
            QueryExecutor = new SqlDomainQueryExecutor(new QueryConfiguration(
                schemaName: config.GetString("schema-name"),
                journalEventsTableName: config.GetString("table-name"),
                metaTableName: config.GetString("metadata-table-name"),
                persistenceIdColumnName: "PersistenceId",
                sequenceNrColumnName: "SequenceNr",
                payloadColumnName: "Payload",
                manifestColumnName: "Manifest",
                timestampColumnName: "Timestamp",
                isDeletedColumnName: "IsDeleted",
                tagsColumnName: "Tags",
                timeout: config.GetTimeSpan("connection-timeout")),
                Context.System.Serialization,
                GetTimestampProvider(config.GetString("timestamp-provider")));
        }

        public override IJournalQueryExecutor QueryExecutor { get; }
    }
}