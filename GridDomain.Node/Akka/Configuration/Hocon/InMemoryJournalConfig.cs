namespace GridDomain.Node.Akka.Configuration.Hocon
{
    internal class InMemoryJournalConfig : IHoconConfig
    {
        private readonly IHoconConfig _eventAdaptersConfig;

        public InMemoryJournalConfig(IHoconConfig eventAdaptersConfig=null)
        {
            _eventAdaptersConfig = eventAdaptersConfig;
        }

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