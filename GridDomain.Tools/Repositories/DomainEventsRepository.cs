using System.IO;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tools.Persistence.SqlPersistence;
using GridDomain.Tools.Repositories;
using Wire;
//using Wire.Extensions;

namespace GridDomain.Tools.Persistence
{
    public class DomainEventsRepository : IRepository<DomainEvent>
    {
        private readonly IRepository<JournalEntry> _rawDataRepo;

        public void Dispose()
        {
        }

        public DomainEventsRepository(IRepository<JournalEntry> rawDataRepo)
        {
            _rawDataRepo = rawDataRepo;
        }

        //Event order matter!!
        public void Save(string id, params DomainEvent[] messages)
        {
            var serializer = new Serializer(new SerializerOptions(true,null,true));
            int counter=0;

            var journalEntries = messages.Select(m =>
            {
                var stream = new MemoryStream();
                serializer.Serialize(m, stream);

                return new JournalEntry()
                {
                    IsDeleted = false,
                    Manifest = m.GetType().AssemblyQualifiedShortName(),
                    Payload =stream.ToArray(),
                    PersistenceId = id,
                    SequenceNr = ++counter,
                    Timestamp = m.CreatedTime.Ticks
                };
            }).ToArray();

            _rawDataRepo.Save(id, journalEntries);
        }

        public DomainEvent[] Load(string id)
        {
            var serializer = new Serializer(new SerializerOptions(true,null,true));
            return
                _rawDataRepo.Load(id)
                    .Select(d => serializer.Deserialize(new MemoryStream(d.Payload)))
                    .Cast<DomainEvent>()
                    .ToArray();
        }

        public static DomainEventsRepository New(string connectionString)
        {
            return new DomainEventsRepository(new RawSqlAkkaPersistenceRepository(connectionString));
        }
    }
}