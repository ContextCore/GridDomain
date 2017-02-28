using Akka.Configuration;
using Akka.Persistence.Sql.Common.Journal;
using Akka.Persistence.SqlServer.Journal;

namespace GridDomain.Node
{
    public class SqlDomainJournal : SqlServerJournal
    {
        public SqlDomainJournal(Config config) : base(config)
        {
            QueryExecutor =
                new SqlDomainQueryExecutor(
                    new QueryConfiguration(config.GetString("schema-name"),
                        config.GetString("table-name"),
                        config.GetString("metadata-table-name"),
                        "PersistenceId",
                        "SequenceNr",
                        "Payload",
                        "Manifest",
                        "Timestamp",
                        "IsDeleted",
                        "Tags",
                        config.GetTimeSpan("connection-timeout")),
                    Context.System.Serialization,
                    GetTimestampProvider(config.GetString("timestamp-provider")));
        }

        public override IJournalQueryExecutor QueryExecutor { get; }
    }
}