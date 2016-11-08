namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class InMemoryJournalConfig : IAkkaConfig
    {
        private readonly IAkkaConfig _eventAdaptersConfig;

        public InMemoryJournalConfig(IAkkaConfig eventAdaptersConfig)
        {
            _eventAdaptersConfig = eventAdaptersConfig;
        }

        public string Build()
        {
            return @"
                 journal {
                    plugin = ""akka.persistence.journal.inmem""
                    inmem {
                            class = ""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""
                            plugin-dispatcher = ""akka.actor.default-dispatcher""
                            "+ _eventAdaptersConfig.Build() + @"
                                }
                        }
";
        }
    }
}