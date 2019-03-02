namespace GridDomain.Node.Akka.Configuration.Hocon
{
    internal class InMemoryJournalConfig : IHoconConfig
    {

        public string Build()
        {
        string config =
            @"akka.persistence.journal.plugin = ""akka.persistence.journal.inmem""
              akka.persistence.journal.inmem.class = ""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""                      
            ";
            return config;
        }
    }
}