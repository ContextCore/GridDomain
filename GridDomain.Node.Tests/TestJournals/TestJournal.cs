using System.Collections.Concurrent;
using System.Collections.Generic;
using Akka.Persistence;
using Akka.Persistence.Journal;
using Akka.Util.Internal;
using GridDomain.Node.Cluster;

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
    }
}