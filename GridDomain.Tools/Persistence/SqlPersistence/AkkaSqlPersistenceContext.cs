using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class AkkaSqlPersistenceContext : DbContext
    {
        public AkkaSqlPersistenceContext(DbContextOptions options) : base(options) {}

        public DbSet<JournalItem> Journal { get; set; } // JournalEntry
        public DbSet<Metadata> Metadatas { get; set; } // Metadata
        public DbSet<SnapshotItem> Snapshots { get; set; } // Snapshots

        public bool IsSqlParameterNull(SqlParameter param)
        {
            SaveChanges();
            SaveChangesAsync();

            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return sqlValue == null || sqlValue == DBNull.Value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddConfiguration(new JournalConfiguration());
            modelBuilder.AddConfiguration(new MetadataConfiguration());
            modelBuilder.AddConfiguration(new SnapshotConfiguration());
        }

    }
}