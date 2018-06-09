using Akka.Configuration;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Hocon
{
    public class DomainEventAdaptersConfig : IHoconConfig
    {
        public Config Build()
        {
            var adaptersConfig = @"
                event-adapters
                {
                    upd = """ + typeof(AkkaDomainEventsAdapter).AssemblyQualifiedShortName() + @"""
                }
                event-adapter-bindings
                {
                    """ + typeof(DomainEvent).AssemblyQualifiedShortName() + @""" = upd
                }";

            return adaptersConfig;
        }
    }
}