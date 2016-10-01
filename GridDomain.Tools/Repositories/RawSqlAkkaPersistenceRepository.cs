using System.Data.Entity.Migrations;
using System.Linq;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories
{
    /// <summary>
    /// Class for reading \ writing data persisted in sql db with wire
    /// Use only in emergency cases by own risk!
    /// For example, when you have different versions of events with same type persisted
    /// for different instance of one aggregate type.
    /// </summary>
    public class RawSqlAkkaPersistenceRepository : IRepository<JournalEntry>
    {
        private readonly string _connectionString;

        public RawSqlAkkaPersistenceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
        }

        public void Save(string id, params JournalEntry[] messages)
        {
            foreach (var m in messages)
                m.PersistenceId = id;

            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                context.Journal.AddOrUpdate(messages);
                context.SaveChanges();
            }
        }

        public JournalEntry[] Load(string id)
        {
            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                return context.Journal.Where(j => j.PersistenceId == id).ToArray();
            }
        }
    }
}