using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Hocon
{
    internal class DomainEventAdaptersConfig : IHoconConfig
    {
        public string Build()
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