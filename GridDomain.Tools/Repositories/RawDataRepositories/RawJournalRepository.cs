using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories.RawDataRepositories
{
    /// <summary>
    /// Class for reading \ writing data persisted in sql db with wire
    /// Use only in emergency cases by own risk!
    /// For example, when you have different versions of events with same type persisted
    /// for different instance of one aggregate type.
    /// </summary>
    public class RawJournalRepository : IRepository<JournalItem>
    {
        private readonly string _connectionString;

        public RawJournalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
        }

        public async Task Save(string id, params JournalItem[] messages)
        {
            foreach (var m in messages)
                m.PersistenceId = id;

            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                context.Journal.AddOrUpdate(messages);
                await context.SaveChangesAsync();
            }
        }

    
        public JournalItem[] Load(string id)
        {
            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                return context.Journal.Where(j => j.PersistenceId == id)
                                      .OrderBy(j => j.SequenceNr)
                                      .ToArray();
            }
        }
    }
}