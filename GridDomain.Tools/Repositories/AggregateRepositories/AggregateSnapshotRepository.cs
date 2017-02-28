using System;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories.RawDataRepositories;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class AggregateSnapshotRepository
    {
        private readonly IConstructAggregates _aggregatesConstructor;
        private readonly string _writeString;

        public AggregateSnapshotRepository(string akkaWriteDbConnectionString, IConstructAggregates aggregatesConstructor)
        {
            _aggregatesConstructor = aggregatesConstructor;
            _writeString = akkaWriteDbConnectionString;
        }

        public async Task<AggregateVersion<T>[]> Load<T>(Guid id) where T : IAggregate
        {
            var serializer = new DomainSerializer();
            using (var repo = new RawSnapshotsRepository(_writeString))
                return (await repo.Load(AggregateActorName.New<T>(id)
                                                          .Name)).Select(s =>
                                                                         {
                                                                             var memento =
                                                                                 (IMemento)
                                                                                     serializer.FromBinary(s.Snapshot,
                                                                                         typeof(IMemento));
                                                                             var aggregate =
                                                                                 (T)
                                                                                     _aggregatesConstructor.Build(typeof(T),
                                                                                         id,
                                                                                         memento);
                                                                             aggregate.ClearUncommittedEvents();
                                                                                 //in case json will call public constructor
                                                                             return new AggregateVersion<T>(aggregate,
                                                                                 s.Timestamp);
                                                                         })
                                                                 .ToArray();
        }

        public async Task Add<T>(T aggregate) where T : IAggregate
        {
            var serializer = new DomainSerializer();

            using (var repo = new RawSnapshotsRepository(_writeString))
            {
                var snapshot = aggregate.GetSnapshot();
                var item = new SnapshotItem
                           {
                               Manifest = snapshot.GetType()
                                                  .AssemblyQualifiedShortName(),
                               PersistenceId = AggregateActorName.New<T>(aggregate.Id)
                                                                 .Name,
                               Snapshot = serializer.ToBinary(snapshot),
                               Timestamp = BusinessDateTime.UtcNow
                           };
                await repo.Save(item.PersistenceId, item);
            }
        }
    }
}