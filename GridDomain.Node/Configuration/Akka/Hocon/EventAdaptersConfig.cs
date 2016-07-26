namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class DomainEventAdaptersConfig : IAkkaConfig
    {
        public string Build()
        {
            var adaptersConfig =
                @"
                event-adapters
                {
                    upd = ""GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.BalanceChangedEventAdapter, GridDomain.Tests.Acceptance""
                }
                event-adapter-bindings
                {
                   # ""GridDomain.EventSourcing.DomainEvent, GridDomain.EventSourcing"" = domainEventsUpgrade
                    ""System.Object"" = upd
                }";

            return adaptersConfig;
        }
    }
}