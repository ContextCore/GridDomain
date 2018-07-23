using System.Linq;
using System.Threading.Tasks;
using GridDomain.Tools.Persistence.SqlPersistence;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tools.Repositories.RawDataRepositories
{
    /// <summary>
    ///     Class for reading \ writing data persisted in sql db with wire
    ///     Use only in emergency cases by own risk!
    ///     For example, when you have different versions of events with same type persisted
    ///     for different instance of one aggregate type.
    /// </summary>
    public class RawJournalRepository : IRepository<JournalItem>
    {
        private readonly DbContextOptions _dbOptions;

        public RawJournalRepository(DbContextOptions dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public void Dispose() {}

        public async Task Save(string aggregateId, params JournalItem[] messages)
        {
            foreach (var m in messages)
                m.PersistenceId = aggregateId;

            using (var context = new AkkaSqlPersistenceContext(_dbOptions))
            {
                context.Journal.AddRange(messages);
                await context.SaveChangesAsync();
            }
        }

        public async Task<JournalItem[]> Load(string persistenceId)
        {
            using (var context = new AkkaSqlPersistenceContext(_dbOptions))
            {
                return await context.Journal.Where(j => j.PersistenceId == persistenceId).OrderBy(j => j.SequenceNr).ToArrayAsync();
            }
        }

        public int TotalCount()
        {
            using (var context = new AkkaSqlPersistenceContext(_dbOptions))
            {
               return (from x in context.Journal select x).Count();
            }
        }
    }
}