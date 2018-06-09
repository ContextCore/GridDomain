using System;
using Akka.Configuration;
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

        public Config Build()
        {
            var actorConfig = @"   
       actor {
             serialize-messages = " + (_serializeMessages ? "on" : "off") + @"
             serialize-creators = " + (_serializeCreators ? "on" : "off") + @"
             serializers {
                        hyp = """ + typeof(DebugHyperionSerializer).AssemblyQualifiedShortName() + @"""
                        json = """ + typeof(DomainEventsJsonAkkaSerializer).AssemblyQualifiedShortName() + @"""
             }
             
             serialization-bindings {
                                   """ + typeof(DomainEvent).AssemblyQualifiedShortName() + @""" = json
                                   """ + typeof(IMemento).AssemblyQualifiedShortName() + @"""    = json
                                  # for local snapshots storage
                                   ""Akka.Persistence.Serialization.Snapshot, Akka.Persistence"" = json
                                   ""System.Object"" = hyp

             }
       }";
            return actorConfig;
        }
    }
}