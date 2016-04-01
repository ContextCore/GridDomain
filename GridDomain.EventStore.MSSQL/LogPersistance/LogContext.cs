using System;
using System.Data.Entity;
using GridDomain.EventStore.MSSQL.Migrations;

namespace GridDomain.EventStore.MSSQL.LogPersistance
{
    public class LogContext:DbContext
    {
        public DbSet<LogRecord> Logs { get; set; }

        public static string DefaultConnectionString =
            @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainLogs;Integrated Security = true";

        [Obsolete("Только для миграций и генерации БД")]
        public LogContext() : base(DefaultConnectionString)
        {

        }
        static LogContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LogContext,Configuration>());
        }

        public LogContext(string connection) : base(connection)
        {

        }
    }
}
