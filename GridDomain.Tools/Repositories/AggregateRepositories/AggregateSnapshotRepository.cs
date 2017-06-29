using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories.RawDataRepositories;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class AggregateSnapshotRepository
    {
        private readonly IConstructAggregates _aggregatesConstructor;
        private readonly DbContextOptions _writeString;

        public AggregateSnapshotRepository(DbContextOptions dbOptions, IConstructAggregates aggregatesConstructor)
        {
            _aggregatesConstructor = aggregatesConstructor;
            _writeString = dbOptions;
        }

        public AggregateSnapshotRepository(string connString,
                                           IConstructAggregates aggregatesConstructor) : this(new DbContextOptionsBuilder().UseSqlServer(connString).
                                                                                                                            Options,
                                                                                              aggregatesConstructor) { }

        public async Task<AggregateVersion<T>[]> Load<T>(Guid id) where T : Aggregate
        {
            var serializer = new DomainSerializer();
            using (var repo = new RawSnapshotsRepository(_writeString))
            {
                return (await repo.Load(AggregateActorName.New<T>(id).
                                                           Name)).Select(s =>
                                                                         {
                                                                             var memento = (IMemento) serializer.FromBinary(s.Snapshot, typeof(IMemento));
                                                                             var aggregate = (T) _aggregatesConstructor.Build(typeof(T), id, memento);
                                                                             return new AggregateVersion<T>(aggregate, s.Timestamp);
                                                                         }).
                                                                  ToArray();
            }
        }

        public async Task Add<T>(T aggregate) where T : IAggregate
        {
            var serializer = new DomainSerializer();

            using (var repo = new RawSnapshotsRepository(_writeString))
            {
                var snapshot = aggregate.GetSnapshot();
                var item = new SnapshotItem
                           {
                               Manifest = snapshot.GetType().
                                                   AssemblyQualifiedShortName(),
                               PersistenceId = AggregateActorName.New<T>(aggregate.Id).
                                                                  Name,
                               Snapshot = serializer.ToBinary(snapshot),
                               Timestamp = BusinessDateTime.UtcNow
                           };
                await repo.Save(item.PersistenceId, item);
            }
        }
    }
}