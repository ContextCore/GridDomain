using System.Linq;
using System.Threading.Tasks;
using GridDomain.Tools.Persistence.SqlPersistence;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tools.Repositories.RawDataRepositories
{
    public class RawSnapshotsRepository : IRepository<SnapshotItem>
    {
        private readonly DbContextOptions _options;

        public RawSnapshotsRepository(DbContextOptions options)
        {
            _options = options;
        }

        public void Dispose() {}

        public async Task Save(string id, params SnapshotItem[] messages)
        {
            foreach (var m in messages)
                m.PersistenceId = id;

            using (var context = new AkkaSqlPersistenceContext(_options))
            {
                context.Snapshots.AddRange(messages);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     Oldest first, order by sequence number ascending
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SnapshotItem[]> Load(string id)
        {
            using (var context = new AkkaSqlPersistenceContext(_options))
            {
                return await context.Snapshots.Where(j => j.PersistenceId == id).OrderBy(i => i.SequenceNr).ToArrayAsync();
            }
        }
    }
}