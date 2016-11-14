using System.Data.Entity.Migrations;
using System.Linq;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories.RawDataRepositories
{
    public class RawSnapshotsRepository : IRepository<SnapshotItem>
    {
        private readonly string _connectionString;

        public RawSnapshotsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
        }
        public void Save(string id, params SnapshotItem[] messages)
        {

            foreach (var m in messages)
                m.PersistenceId = id;

            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                context.Snapshots.AddOrUpdate(messages);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Oldest first, order by sequence number ascending
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SnapshotItem[] Load(string id)
        {
            using (var context = new AkkaSqlPersistenceContext(_connectionString))
            {
                return context.Snapshots.Where(j => j.PersistenceId == id)
                              .OrderBy(i => i.SequenceNr)
                              .ToArray();
            }
        }

    }
}