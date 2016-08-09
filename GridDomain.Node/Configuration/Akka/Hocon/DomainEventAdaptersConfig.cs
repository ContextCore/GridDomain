using System;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Akka.Hocon
{

    public static class TypeExtensions
    {
        public static string ToAssemblyQualifiedShortName(this Type type)
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
                    upd = """+ typeof(AkkaDomainEventsAdapter).ToAssemblyQualifiedShortName() +@"""
                }
                event-adapter-bindings
                {
                    """ + typeof(DomainEvent).ToAssemblyQualifiedShortName() + @""" = upd
                }";

            return adaptersConfig;
        }
    }


}