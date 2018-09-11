using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories.RawDataRepositories;

namespace GridDomain.Tools.Repositories.SnapshotRepositories
{

    public static class DataSnapshotRepository
    {
        public static DataSnapshotRepository<T> New<T>(string connectionString) where T : class, ISnapshot
        {
             return new DataSnapshotRepository<T>(new RawSnapshotsRepository(connectionString));
        }
    }
    
    public class DataSnapshotRepository<T>  : IRepository<T> where T: class, ISnapshot
    {
        private readonly IRepository<SnapshotItem> _snapItemRepository;
        private readonly DomainSerializer _domainSerializer;

        
        public DataSnapshotRepository(IRepository<SnapshotItem> snapItemRepository)
        {
            _snapItemRepository = snapItemRepository;
            _domainSerializer = new DomainSerializer();
        }

        
        public void Dispose()
        {
            
        }

        public Task Save(string aggregateId, params T[] messages)
        {
            int seqNum = 0;
            return _snapItemRepository.Save(aggregateId,
                                            messages.Select(s => new SnapshotItem()
                                                                {
                                                                    Manifest = typeof(T).FullName,
                                                                    PersistenceId = aggregateId,
                                                                    SequenceNr =  ++seqNum,
                                                                    Snapshot = _domainSerializer.ToBinary(s),
                                                                    Timestamp = BusinessDateTime.Now
                                                                })
                                                    .ToArray());
        }

        public async Task<T[]> Load(string persistenceId)
        {
            var rawData = await _snapItemRepository.Load(persistenceId);
            return rawData.Select(d => _domainSerializer.FromBinary(d.Snapshot, typeof(T)))
                          .Cast<T>()
                          .ToArray();
        }
    }
}