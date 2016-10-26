using System;
using System.IO;
using System.Linq;
using System.Text;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Tools.Persistence.SqlPersistence;
using Newtonsoft.Json;
using Wire;

//using Wire.Extensions;

namespace GridDomain.Tools.Repositories
{
    public class DomainEventsRepository : IRepository<DomainEvent>
    {
        private readonly IRepository<JournalItem> _rawDataRepo;

        public void Dispose()
        {
        }

        public DomainEventsRepository(IRepository<JournalItem> rawDataRepo)
        {
            _rawDataRepo = rawDataRepo;
        }

        //Event order matter!!
        public void Save(string id, params DomainEvent[] messages)
        {
            long counter=0;

            var journalEntries = messages.Select(m =>
            {
                var json = JsonConvert.SerializeObject(m, DomainEventSerialization.GetDefaultSettings());
                return new JournalItem(id,
                                       ++counter,
                                       false,
                                       m.GetType().AssemblyQualifiedShortName(),
                                       m.CreatedTime,
                                       "",
                                       Encoding.Unicode.GetBytes(json));
            }).ToArray();

            _rawDataRepo.Save(id, journalEntries);
        }

        public DomainEvent[] Load(string id)
        {
            var serializer = new Serializer(new SerializerOptions(true,true));
            return
                _rawDataRepo.Load(id)
                    .Select(d =>
                    {
                        try
                        {
                            return serializer.Deserialize(new MemoryStream(d.Payload));
                        }
                        catch (NullReferenceException ex)
                        {
                            throw new PersistanceFailureException(d, ex);
                        }
                    })
                    .Cast<DomainEvent>()
                    .ToArray();
        }

        public static DomainEventsRepository New(string connectionString)
        {
            return new DomainEventsRepository(new RawSqlAkkaPersistenceRepository(connectionString));
        }
    }
}