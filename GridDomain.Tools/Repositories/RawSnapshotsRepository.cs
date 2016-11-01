using System.Data.Entity.Migrations;
using System.Linq;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories
{
    public class RawSnapshotsRepository : IRepository<Snapshot>
    {
        private readonly string _connectionString;

        public RawSnapshotsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
        }
        public void Save(string id, params Snapshot[] messages)
        {

            foreach (var m in messages)
                m.PersistenceId = id;

            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                context.Snapshots.AddOrUpdate(messages);
                context.SaveChanges();
            }
        }

        public Snapshot[] Load(string id)
        {
            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                return context.Snapshots.Where(j => j.PersistenceId == id).ToArray();
            }
        }

    }
}