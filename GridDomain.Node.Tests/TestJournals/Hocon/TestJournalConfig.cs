using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Tests.TestJournals.Hocon
{
    public class TestJournalConfig : IHoconConfig
    {
        public const string JournalId = "akka.persistence.journal.inmem";
        public string Build()
        {
        string config =
            @"akka.persistence.journal.plugin = ""akka.persistence.journal.inmem""
              akka.persistence.journal.inmem.class = """+typeof(TestJournal).AssemblyQualifiedShortName()+@"""
              akka.persistence.query.journal.plugin = """+TestReadJournal.Identifier+@"""
              akka.persistence.query.journal.test.class = """+typeof(TestReadJournalProvider).AssemblyQualifiedShortName()+@"""
              akka.persistence.publish-plugin-commands = on";
            return config;
        }
    }
}