using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class AkkaSqlPersistenceContext : DbContext,
                                             IAkkaSqlPersistenceContext
    {
        static AkkaSqlPersistenceContext()
        {
            Database.SetInitializer<AkkaSqlPersistenceContext>(null);
        }

        public AkkaSqlPersistenceContext(string connectionString) : base(connectionString) {}

        public DbSet<JournalItem> Journal { get; set; } // JournalEntry
        public DbSet<Metadata> Metadatas { get; set; } // Metadata
        public DbSet<SnapshotItem> Snapshots { get; set; } // Snapshots

        public bool IsSqlParameterNull(SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return sqlValue == null || sqlValue == DBNull.Value;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new JournalConfiguration());
            modelBuilder.Configurations.Add(new MetadataConfiguration());
            modelBuilder.Configurations.Add(new SnapshotConfiguration());
        }

        public static DbModelBuilder CreateModel(DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new JournalConfiguration(schema));
            modelBuilder.Configurations.Add(new MetadataConfiguration(schema));
            modelBuilder.Configurations.Add(new SnapshotConfiguration(schema));
            return modelBuilder;
        }
    }
}