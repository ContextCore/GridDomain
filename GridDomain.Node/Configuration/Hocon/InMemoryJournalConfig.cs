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

        public Config Build()
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