using System;
using Akka.Actor;
using GridDomain.Node.Configuration;
using Serilog.Events;

namespace GridDomain.Node.Persistence.Sql
{
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateSystem(this NodeConfiguration conf, ISqlNodeDbConfiguration cfg)
        {
            return ActorSystem.Create(conf.Name,
                                      ToStandAloneSystemConfig(new ActorSystemConfigBuilder(), cfg, conf.LogLevel, conf.Address)
                                          .Build());
        }

        public static IActorSystemConfigBuilder ToStandAloneSystemConfig(this IActorSystemConfigBuilder actorSystemConfigBuilder,
                                                                         ISqlNodeDbConfiguration persistence,
                                                                         LogEventLevel logEventLevel,
                                                                         INodeNetworkAddress nodeNetworkAddress,
                                                                         bool serializeMessagesCreators = false)
        {
            return actorSystemConfigBuilder
                   .Log(logEventLevel)
                   .DomainSerialization(serializeMessagesCreators)
                   .RemoteActorProvider()
                   .Remote(nodeNetworkAddress)
                   .SqlPersistence(persistence);
        }
    }
}