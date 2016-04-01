using System;
using System.Data.Entity;
using GridDomain.EventStore.MSSQL.Migrations;

namespace GridDomain.EventStore.MSSQL.LogPersistance
{
    public class LogContext : DbContext
    {
        public static string DefaultConnectionString =
            @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainLogs;Integrated Security = true";

        static LogContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LogContext, Configuration>());
        }

        [Obsolete("Только для миграций и генерации БД")]
        public LogContext() : base(DefaultConnectionString)
        {
        }

        public LogContext(string connection) : base(connection)
        {
        }

        public DbSet<LogRecord> Logs { get; set; }
    }
}