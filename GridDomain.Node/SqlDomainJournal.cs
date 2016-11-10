using Akka.Configuration;
using Akka.Persistence.Sql.Common.Journal;
using Akka.Persistence.SqlServer.Journal;
using Akka.Serialization;

public class SqlDomainJournal : SqlServerJournal
{
    public SqlDomainJournal(Config journalConfig) : base(journalConfig)
    {
    }
}


    public class SqlDomainQueryExecutor : SqlServerQueryExecutor
    {
        public SqlDomainQueryExecutor(QueryConfiguration configuration, Serialization serialization, ITimestampProvider timestampProvider) : base(configuration, serialization, timestampProvider)
        {
        var allEventColumnNames = $@"
                e.{Configuration.PersistenceIdColumnName} as PersistenceId, 
                e.{Configuration.SequenceNrColumnName} as SequenceNr, 
                e.{Configuration.TimestampColumnName} as Timestamp, 
                e.{Configuration.IsDeletedColumnName} as IsDeleted, 
                e.{Configuration.ManifestColumnName} as Manifest, 
                e.{Configuration.PayloadColumnName} as Payload";

                ByPersistenceIdSql =
                    $@"
                       SELECT {allEventColumnNames}
                       FROM {Configuration.FullJournalTableName} e
                       WHERE e.{Configuration.PersistenceIdColumnName} = @PersistenceId
                       AND e.{Configuration.SequenceNrColumnName} BETWEEN @FromSequenceNr AND @ToSequenceNr
                       ORDER BY SequenceNr
                       ASC;";

        }
        protected override string ByPersistenceIdSql { get;}
    }
