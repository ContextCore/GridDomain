using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public interface IAkkaSqlPersistenceContext : IDisposable
    {
        DbSet<JournalItem> Journal { get; set; } // JournalEntry
        DbSet<Metadata> Metadatas { get; set; } // Metadata
        DbSet<SnapshotItem> Snapshots { get; set; } // Snapshots

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}