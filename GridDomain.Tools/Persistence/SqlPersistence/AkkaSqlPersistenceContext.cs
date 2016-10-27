
namespace GridDomain.Tools.Persistence.SqlPersistence
{

  
    public class AkkaSqlPersistenceContext : System.Data.Entity.DbContext, IAkkaSqlPersistenceContext
    {
        public System.Data.Entity.DbSet<JournalItem> Journal { get; set; } // JournalEntry
        public System.Data.Entity.DbSet<Metadata> Metadatas { get; set; } // Metadata
        public System.Data.Entity.DbSet<Snapshot> Snapshots { get; set; } // Snapshots

        static AkkaSqlPersistenceContext()
        {
            System.Data.Entity.Database.SetInitializer<AkkaSqlPersistenceContext>(null);
        }

        public AkkaSqlPersistenceContext()
            : base("Name=SqlJournal")
        {
        }

        public AkkaSqlPersistenceContext(string connectionString)
            : base(connectionString)
        {
        }

        public AkkaSqlPersistenceContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public AkkaSqlPersistenceContext(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public AkkaSqlPersistenceContext(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public bool IsSqlParameterNull(System.Data.SqlClient.SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as System.Data.SqlTypes.INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == System.DBNull.Value);
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new JournalConfiguration());
            modelBuilder.Configurations.Add(new MetadataConfiguration());
            modelBuilder.Configurations.Add(new SnapshotConfiguration());
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new JournalConfiguration(schema));
            modelBuilder.Configurations.Add(new MetadataConfiguration(schema));
            modelBuilder.Configurations.Add(new SnapshotConfiguration(schema));
            return modelBuilder;
        }
    }
}

