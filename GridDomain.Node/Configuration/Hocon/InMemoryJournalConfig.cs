namespace GridDomain.Node.Configuration.Hocon
{
    internal class InMemoryJournalConfig : INodeConfig
    {
        private readonly INodeConfig _eventAdaptersConfig;

        public InMemoryJournalConfig(INodeConfig eventAdaptersConfig)
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
                            " + _eventAdaptersConfig.Build() + @"
                                }
                        }
";
        }
    }
}