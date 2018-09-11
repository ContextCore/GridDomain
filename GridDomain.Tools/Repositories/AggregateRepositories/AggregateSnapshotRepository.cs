using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Hocon;
using GridDomain.Node.Serializers;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories.RawDataRepositories;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class AggregateSnapshotRepository
    {
        private readonly IAggregateFactory _aggregatesConstructor;
        private readonly DbContextOptions option;
        private readonly ISnapshotFactory _snapshotFactoryConstructor;

        public static AggregateSnapshotRepository New(string options)
        {
            return new AggregateSnapshotRepository(options, AggregateFactory.Default, AggregateFactory.Default);
        }
        
        public static AggregateSnapshotRepository New(DbContextOptions options)
        {
            return new AggregateSnapshotRepository(options, AggregateFactory.Default, AggregateFactory.Default);
        }
        
        public static AggregateSnapshotRepository New<T>(DbContextOptions options, T factory) where T: class, IAggregateFactory,
            ISnapshotFactory
        {
            return new AggregateSnapshotRepository(options, factory, factory);
        }
        
        public static AggregateSnapshotRepository New<T>(string options, T factory) where T: class, IAggregateFactory,
            ISnapshotFactory
        {
            return new AggregateSnapshotRepository(options, factory, factory);
        }
        
        public AggregateSnapshotRepository(DbContextOptions dbOptions, 
                                           IAggregateFactory aggregatesConstructor,
                                           ISnapshotFactory snapshotFactoryConstructor)
        {
            _snapshotFactoryConstructor = snapshotFactoryConstructor;
            _aggregatesConstructor = aggregatesConstructor;
            option = dbOptions;
        }

        public AggregateSnapshotRepository(string connString,
                                           IAggregateFactory aggregatesConstructor,
                                           ISnapshotFactory snapshotFactoryConstructor,
                                           int serializerId = 21
                                           ) : this(new DbContextOptionsBuilder().UseSqlServer(connString).Options,
                                                                                              aggregatesConstructor,
                                                    snapshotFactoryConstructor) { }

        public async Task<Version<T>[]> Load<T>(string id) where T : class, IAggregate
        {
            var snapshotType = _snapshotFactoryConstructor.GetSnapshot(_aggregatesConstructor.BuildEmpty<T>())
                                                    .GetType();

            var serializer = new DomainSerializer();
            
            using (var snapshotItemsRepo = new RawSnapshotsRepository(option))
            {
                return (await snapshotItemsRepo.Load(EntityActorName.New<T>(id).
                                                           Name)).Select(s =>
                                                                         {
                                                                             var memento = (ISnapshot) serializer.FromBinary(s.Snapshot, snapshotType);
                                                                             var aggregate = (T) _aggregatesConstructor.Build(typeof(T), id, memento);
                                                                             return new Version<T>(aggregate, s.Timestamp);
                                                                         }).
                                                                  ToArray();
            }
        }
        
        

        public async Task Add<T>(T aggregate) where T : class, IAggregate
        {
            using (var repo = new RawSnapshotsRepository(option))
            {
                var snapshot = _snapshotFactoryConstructor.GetSnapshot(aggregate);
                var item = new SnapshotItem
                           {
                               Manifest = snapshot.GetType().AssemblyQualifiedShortName(),
                               PersistenceId = EntityActorName.New<T>(aggregate.Id).Name,
                               Snapshot = new DomainSerializer().ToBinary(snapshot),
                               Timestamp = BusinessDateTime.UtcNow,
                               
                           };
                await repo.Save(item.PersistenceId, item);
            }
        }
    }
}