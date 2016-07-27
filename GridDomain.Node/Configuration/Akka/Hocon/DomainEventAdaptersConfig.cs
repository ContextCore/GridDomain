namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class DomainEventAdaptersConfig : IAkkaConfig
    {
        public string Build()
        {
            var type = typeof(AkkaDomainEventsAdapter);
            var adaptersConfig =
                @"
                event-adapters
                {
                    upd = """+ type.FullName+", " + type.Assembly+@"""
                }
                event-adapter-bindings
                {
                    ""GridDomain.EventSourcing.DomainEvent, GridDomain.EventSourcing"" = upd
                    #""System.Object"" = upd
                }";

            return adaptersConfig;
        }
    }
}