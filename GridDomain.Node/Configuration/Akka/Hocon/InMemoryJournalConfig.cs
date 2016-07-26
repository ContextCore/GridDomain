namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class InMemoryJournalConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akka;

        public InMemoryJournalConfig(AkkaConfiguration akka)
        {
            _akka = akka;
        }

        public string Build()
        {
            return BuildPersistenceJournalConfig(_akka);
        }

        public static string BuildPersistenceJournalConfig(AkkaConfiguration akkaConf)
        {
            var persistenceJournalConfig = @"
            journal {
                    inmem {
                            class = ""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""
                            plugin-dispatcher = ""akka.actor.default-dispatcher""

                            event-adapters
                            {
                                domainEventsUpgrade = ""GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.BalanceChangedEventAdapter, GridDmoin.Tests.Acceptance""
                            }
                   
                            event-adapter-bindings
                            {
                                ""GridDomain.EventSourcing.DomainEvent, GridDomain.EventSourcing"" = domainEventsUpgrade
                            }
                    }
            }
";
            return persistenceJournalConfig;
        }
    }
}