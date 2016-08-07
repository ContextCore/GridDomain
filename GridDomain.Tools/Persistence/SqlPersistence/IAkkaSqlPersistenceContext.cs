namespace GridDomain.Tools.SqlPersistence
{
    public interface IAkkaSqlPersistenceContext : System.IDisposable
    {
        System.Data.Entity.DbSet<JournalEntry> Journals { get; set; } // JournalEntry
        System.Data.Entity.DbSet<Metadata> Metadatas { get; set; } // Metadata
        System.Data.Entity.DbSet<Snapshot> Snapshots { get; set; } // Snapshots

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
    }
}