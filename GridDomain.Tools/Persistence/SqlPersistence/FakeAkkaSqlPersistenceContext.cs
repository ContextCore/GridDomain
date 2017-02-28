using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class FakeAkkaSqlPersistenceContext : IAkkaSqlPersistenceContext
    {
        public FakeAkkaSqlPersistenceContext()
        {
            Journal = new FakeDbSet<JournalItem>("PersistenceId", "SequenceNr");
            Metadatas = new FakeDbSet<Metadata>("PersistenceId", "SequenceNr");
            Snapshots = new FakeDbSet<SnapshotItem>("PersistenceId", "SequenceNr");
        }

        public int SaveChangesCount { get; private set; }
        public DbSet<JournalItem> Journal { get; set; }
        public DbSet<Metadata> Metadatas { get; set; }
        public DbSet<SnapshotItem> Snapshots { get; set; }

        public int SaveChanges()
        {
            ++SaveChangesCount;
            return 1;
        }

        public Task<int> SaveChangesAsync()
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(() => 1);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(() => 1, cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {}
    }
}