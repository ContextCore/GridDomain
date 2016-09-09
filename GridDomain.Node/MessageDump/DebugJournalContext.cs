using System.Data.Entity;

namespace GridDomain.Node.MessageDump
{
    public class DebugJournalContext: DbContext
    {
        public DebugJournalContext(string connectionString): base(connectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbg");
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DebugJournalEntry> Journal { get; set; }
        public DbSet<JournalEntryFault> SosJournal { get; set; }
    }
}