using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;

namespace GridDomain.Node.Tests.TestJournals
{
    public class TestReadJournalProvider : IReadJournalProvider
    {
        private readonly ExtendedActorSystem _system;
        private readonly Config _config;

        public TestReadJournalProvider(ExtendedActorSystem system, Config config)
        {
            _system = system;
        }

        public IReadJournal GetReadJournal()
        {
            return new TestReadJournal(TestJournal.SharedJournals[_system.GetAddress().ToString()]);
        }
    }
}