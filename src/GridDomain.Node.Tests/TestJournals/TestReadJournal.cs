using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors;
using GridDomain.Query;
using Xunit;

namespace GridDomain.Node.Tests.TestJournals
{
    public class TestReadJournal : ICurrentPersistenceIdsQuery, ICurrentEventsByPersistenceIdQuery, IEventsByTagQuery
    {
        public static string Identifier = "akka.persistence.query.journal.test";
        private readonly ConcurrentDictionary<string, LinkedList<IPersistentRepresentation>> _memory;

        public TestReadJournal(TestJournal journal)
        {
            _memory = journal.Memory;
        }

        public Source<string, NotUsed> CurrentPersistenceIds()
        {
            return Source.FromEnumerator(() => _memory.Keys.GetEnumerator())
                .MapMaterializedValue(_ => NotUsed.Instance)
                .Named("AllPersistenceIds");
        }

        Source<EventEnvelope, NotUsed> ICurrentEventsByPersistenceIdQuery.CurrentEventsByPersistenceId(
            string persistenceId, long fromSequenceNr, long toSequenceNr)
        {
            return Source.FromEnumerator(() => _memory[persistenceId].SkipWhile(e => e.SequenceNr < fromSequenceNr)
                    .TakeWhile(e => e.SequenceNr <= toSequenceNr)
                    .Select(e => e.Payload as EventEnvelope)
                    .GetEnumerator())
                .MapMaterializedValue(_ => NotUsed.Instance)
                .Named("CurrentEventsByPersistenceId-" + persistenceId);
        }

        public Source<EventEnvelope, NotUsed> EventsByTag(string tag, Offset offset)
        {
            return Source.FromEnumerator(() => _memory.Values.SelectMany(v => v)
                    .Where(v =>
                    {
                        if (!(v.Payload is IDomainEvent d)) return false;
                        return d.Source.Name == tag;
                    }).Select(d =>
                    {
                        var evt = d.Payload as IDomainEvent; 
                        return new EventEnvelope(Offset.NoOffset(), evt.Source.Id,evt.Version,evt);
                    }).GetEnumerator())
                .MapMaterializedValue(_ => NotUsed.Instance)
                .Named("EventsByTag");
        }
    }
}