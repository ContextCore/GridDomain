using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon
{
    internal class InMemoryJournalConfig : IHoconConfig
    {
        private readonly IHoconConfig _eventAdaptersConfig;

        public InMemoryJournalConfig(IHoconConfig eventAdaptersConfig)
        {
            _eventAdaptersConfig = eventAdaptersConfig;
        }

        public string Build()
        {
            var build = _eventAdaptersConfig.Build();
            string config = @"
                 akka.persistence.journal {
                    plugin = ""akka.persistence.journal.inmem""
                    inmem {
                            class = ""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""
                            plugin-dispatcher = ""akka.actor.default-dispatcher""
                            " + build.ToString() + @"
                                }
                        }
";
            return config;
        }
    }
}