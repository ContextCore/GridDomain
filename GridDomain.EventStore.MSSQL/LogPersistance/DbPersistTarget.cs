using System.Diagnostics;
using System.Linq;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace GridDomain.EventStore.MSSQL.LogPersistance
{
    [Target("DbPersist")]
    public sealed class DbPersistTarget : TargetWithLayout
    {
        public static readonly Stopwatch DebugTimer = new Stopwatch();

        public DbPersistTarget()
        {
            Layout = "${threadid}";
        }

        [RequiredParameter]
        public string ConnectionString { get; set; }


        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            using (var ts = new TransactionScope())
            {
                using (var context = new LogContext(ConnectionString))
                {
                    var logRecords = logEvents.Select(l => ConvertToLogRecord(l.LogEvent));
                    context.BulkInsert(logRecords, 1000);
                    context.SaveChanges();
                }
                ts.Complete();
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            using (var ts = new TransactionScope())
            {
                using (var context = new LogContext(ConnectionString))
                {
                    context.Logs.Add(ConvertToLogRecord(logEvent));
                    context.SaveChanges();
                }
                ts.Complete();
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(new[] {logEvent});
        }

        private LogRecord ConvertToLogRecord(LogEventInfo logInfo)
        {
            return new LogRecord
            {
                Message = logInfo.FormattedMessage,
                Level = logInfo.Level.ToString(),
                Logger = logInfo.LoggerName,
                Logged = logInfo.TimeStamp,
                ThreadId = int.Parse(Layout.Render(logInfo)),
                TicksFromAppStart = DebugTimer.ElapsedTicks
            };
        }
    }
}