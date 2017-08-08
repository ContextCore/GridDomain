using System;
using System.Collections.Immutable;
using System.Data.Common;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Akka.Persistence;
using Akka.Persistence.Sql.Common.Journal;
using Akka.Persistence.SqlServer.Journal;
using Akka.Serialization;
using Serilog;

namespace GridDomain.Node
{
    // <summary>
    // Detects the exceptions caused by SQL Azure transient failures.
    // </summary>

    public class SqlDomainQueryExecutor : SqlServerQueryExecutor
    {
        private readonly ILogger _log = Log.Logger.ForContext<SqlDomainJournal>();

        public SqlDomainQueryExecutor(QueryConfiguration configuration,
                                      Serialization serialization,
                                      ITimestampProvider timestampProvider)
            : base(configuration, serialization, timestampProvider)
        {
            var allEventColumnNames =
                $@"
                e.{Configuration.PersistenceIdColumnName} as PersistenceId, 
                e.{Configuration
                    .SequenceNrColumnName} as SequenceNr, 
                e.{Configuration.TimestampColumnName} as Timestamp, 
                e.{Configuration
                    .IsDeletedColumnName} as IsDeleted, 
                e.{Configuration.ManifestColumnName} as Manifest, 
                e.{Configuration
                    .PayloadColumnName} as Payload";

            ByPersistenceIdSql =
                $@"
                       SELECT {allEventColumnNames}
                       FROM {Configuration
                    .FullJournalTableName} e
                       WHERE e.{Configuration.PersistenceIdColumnName} = @PersistenceId
                       AND e.{Configuration
                    .SequenceNrColumnName} BETWEEN @FromSequenceNr AND @ToSequenceNr
                       ORDER BY SequenceNr ASC;";
        }

        protected override string ByPersistenceIdSql { get; }

        private async Task RetryAsync(Func<Task> act, int maxCount = 3)
        {
            Exception ex;
            do
            {
                try
                {
                    await act();
                    return;
                }
                catch (Exception e)
                {
                    ex = e;
                    _log.Warning(e, "Got error on trying to execute a sql journal method, will retry");
                }
            }
            while (SqlAzureRetriableExceptionDetector.ShouldRetryOn(ex) && --maxCount > 0);

            _log.Error(ex, "Got fatal error trying to execute a sql journal method");
            ExceptionDispatchInfo.Capture(ex).Throw();
        }

        private async Task<T> RetryAsync<T>(Func<Task<T>> act, int maxCount = 3)
        {
            Exception ex;
            do
            {
                try
                {
                    return await act();
                }
                catch (Exception e)
                {
                    ex = e;
                    _log.Warning(e, "Got error on trying to execute a sql journal method, will retry");
                }
            }
            while (SqlAzureRetriableExceptionDetector.ShouldRetryOn(ex) && --maxCount > 0);
            _log.Error(ex, "Got fatal error trying to execute a sql journal method");
            ExceptionDispatchInfo.Capture(ex).Throw();
            //never reach this statement
            throw new InvalidOperationException();
        }

        private void Retry(Action action, int maxCount = 3)
        {
            Exception ex;
            do
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception e)
                {
                    ex = e;
                    _log.Warning(e, "Got error on trying to execute a sql journal method, will retry");
                }
            }
            while (SqlAzureRetriableExceptionDetector.ShouldRetryOn(ex) && --maxCount > 0);

            _log.Error(ex, "Got fatal error trying to execute a sql journal method");
            ExceptionDispatchInfo.Capture(ex).Throw();
            //never reach this statement
            throw new InvalidOperationException();
        }

        public override async Task InsertBatchAsync(DbConnection connection,
                                                    CancellationToken cancellationToken,
                                                    WriteJournalBatch write)
        {
            await RetryAsync(() => base.InsertBatchAsync(connection, cancellationToken, write));
        }

        public override async Task SelectByPersistenceIdAsync(DbConnection connection,
                                                              CancellationToken cancellationToken,
                                                              string persistenceId,
                                                              long fromSequenceNr,
                                                              long toSequenceNr,
                                                              long max,
                                                              Action<IPersistentRepresentation> callback)
        {
            await
                RetryAsync(
                           () =>
                               base.SelectByPersistenceIdAsync(connection,
                                                               cancellationToken,
                                                               persistenceId,
                                                               fromSequenceNr,
                                                               toSequenceNr,
                                                               max,
                                                               callback));
        }

        protected override void WriteEvent(DbCommand command, IPersistentRepresentation e, IImmutableSet<string> tags)
        {
            Retry(() => base.WriteEvent(command, e, tags));
        }

        public override async Task<long> SelectHighestSequenceNrAsync(DbConnection connection,
                                                                      CancellationToken cancellationToken,
                                                                      string persistenceId)
        {
            return await RetryAsync(() => base.SelectHighestSequenceNrAsync(connection, cancellationToken, persistenceId));
        }

        public override async Task<ImmutableArray<string>> SelectAllPersistenceIdsAsync(DbConnection connection,
                                                                                        CancellationToken cancellationToken)
        {
            return await RetryAsync(() => base.SelectAllPersistenceIdsAsync(connection, cancellationToken));
        }

        public override async Task<long> SelectByTagAsync(DbConnection connection,
                                                          CancellationToken cancellationToken,
                                                          string tag,
                                                          long fromOffset,
                                                          long toOffset,
                                                          long max,
                                                          Action<ReplayedTaggedMessage> callback)
        {
            return
                await
                    RetryAsync(
                               () => base.SelectByTagAsync(connection, cancellationToken, tag, fromOffset, toOffset, max, callback));
        }
    }
}