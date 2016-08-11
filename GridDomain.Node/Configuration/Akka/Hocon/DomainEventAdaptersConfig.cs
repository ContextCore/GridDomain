using System;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Akka.Hocon
{

    public static class TypeExtensions
    {
        public static string AssemblyQualifiedShortName(this Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }
    }
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