using GridDomain.EventSourcing;

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
                    upd = """+ typeof(AkkaDomainEventsAdapter).AssemblyQualifiedShortName() +@"""
                }
                event-adapter-bindings
                {
                    """ + typeof(DomainEvent).AssemblyQualifiedShortName() + @""" = upd
                }";

            return adaptersConfig;
        }
    }


}