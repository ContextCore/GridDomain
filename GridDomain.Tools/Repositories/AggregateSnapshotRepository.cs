using System;
using System.Linq;
using System.Text;
using CommonDomain;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tools.Persistence.SqlPersistence;
using Newtonsoft.Json;

namespace GridDomain.Tools.Repositories
{
    public class AggregateVersion<T> where T : IAggregate
    {
        public AggregateVersion(T aggregate, DateTime createdAt)
        {
            Aggregate = aggregate;
            CreatedAt = createdAt;
        }

        public T Aggregate { get; }
        public DateTime CreatedAt { get; }
    }

    public class AggregateSnapshotRepository
    {
        private readonly string _writeString;

    
        public AggregateSnapshotRepository(string akkaWriteDbConnectionString)
        {
            _writeString = akkaWriteDbConnectionString;
        }

        public AggregateVersion<T>[] Load<T>(Guid id) where T:IAggregate
        {

            using (var repo = new RawSnapshotsRepository(_writeString))
                return repo.Load(AggregateActorName.New<T>(id).Name)
                           .Select(s =>
                           {
                               var jsonString = Encoding.Unicode.GetString(s.Snapshot);
                               var aggregate = JsonConvert.DeserializeObject<T>(jsonString, DomainSerializer.GetDefaultSettings());
                               aggregate.ClearUncommittedEvents(); //in case json will call public constructor
                               return new AggregateVersion<T>(aggregate, s.Timestamp);
                           }).ToArray();
        }

        public void Add<T>(T aggregate) where T : IAggregate
        {
            using (var repo = new RawSnapshotsRepository(_writeString))
            {
                var jsonString = JsonConvert.SerializeObject(aggregate, DomainSerializer.GetDefaultSettings());
                var item = new SnapshotItem()
                {
                    Manifest = typeof(T).AssemblyQualifiedShortName(),
                    PersistenceId = AggregateActorName.New<T>(aggregate.Id).Name,
                    Snapshot = Encoding.Unicode.GetBytes(jsonString),
                    Timestamp = BusinessDateTime.UtcNow
                };
                repo.Save(item.PersistenceId,item);
            }
        }
    }
}