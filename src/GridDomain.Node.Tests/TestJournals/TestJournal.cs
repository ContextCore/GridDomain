using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Journal;
using Akka.Util.Internal;
using GridDomain.Aggregates;

namespace GridDomain.Node.Tests.TestJournals
{
    public class TestJournal : MemoryJournal
    {
        public static ConcurrentDictionary<string, TestJournal> SharedJournals = new ConcurrentDictionary<string, TestJournal>();
        public ConcurrentDictionary<string,LinkedList<IPersistentRepresentation>> Memory => Messages;

        public TestJournal()
        {
            var key = Context.System.GetAddress().ToString();
            SharedJournals.AddOrSet(key, this);
        }

        protected override Task<IImmutableList<Exception>> WriteMessagesAsync(IEnumerable<AtomicWrite> messages)
        {
            foreach (var persistentRepresentation in messages.Where(w => w.Payload is IImmutableList<IPersistentRepresentation>)
                                                             .SelectMany(w => w.Payload as IImmutableList<IPersistentRepresentation>))
            {
                if (!Messages.TryGetValue(persistentRepresentation.PersistenceId, out var list)) continue;
                
                if (list.Any(e =>
                    e.SequenceNr == persistentRepresentation.SequenceNr))
                    throw new DublicateEventPersistException(persistentRepresentation.PersistenceId, persistentRepresentation.SequenceNr);
            }
            return base.WriteMessagesAsync(messages);
        }

        public class DublicateEventPersistException : Exception
        {
            public string AggregateAddress { get; }
            public long SequenceNr { get; }

            public DublicateEventPersistException(string aggregateAddress, long SequenceNr)
            {
                AggregateAddress = aggregateAddress;
                this.SequenceNr = SequenceNr;
            }
        }
    }
}