using System;
using Akka.Configuration;
using Akka.Serialization;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Serializers;

namespace GridDomain.Node.Configuration.Hocon
{

    public class SerializersConfig : IHoconConfig
    {
        private readonly bool _serializeMessages;
        private readonly bool _serializeCreators;

        public SerializersConfig(bool serializeMessages = false, bool serializeCreators = false)
        {
            _serializeCreators = serializeCreators;
            _serializeMessages = serializeMessages;
        }

        public string Build()
        {
            string actorConfig = @"   
       akka.actor {
             serialize-messages = " + (_serializeMessages ? "on" : "off") + @"
             serialize-creators = " + (_serializeCreators ? "on" : "off") + @"
             serializers {
                        hyperion = """ + typeof(DebugHyperionSerializer).AssemblyQualifiedShortName() + @"""
                        domain = """ + typeof(DomainEventsJsonAkkaSerializer).AssemblyQualifiedShortName() + @"""
             }
             
             serialization-bindings {
                                   """ + typeof(DomainEvent).AssemblyQualifiedShortName() + @""" = domain
                                   """ + typeof(IMemento).AssemblyQualifiedShortName() + @"""    = domain
                                  # for local snapshots storage
                                   """+ typeof(Akka.Persistence.Serialization.Snapshot).AssemblyQualifiedShortName() + @""" = domain
                                   ""System.Object"" = hyperion
                                   """+ typeof(Object).AssemblyQualifiedShortName() + @""" = hyperion

             }
       }";
            return actorConfig;
        }
    }
}