using System;
using Akka.Actor;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Persistence.Sql
{

    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateSystem(this NodeConfiguration conf, ISqlNodeDbConfiguration cfg)
        {
            return ActorSystem.Create(conf.Name, conf.ToStandAloneSystemConfig(cfg).Build());
        }

        public static ActorSystemConfigBuilder ToStandAloneSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence, bool serializeMessagesCreators = false)
        {
            return ActorSystemConfigBuilder.New()
                                     .Log(conf.LogLevel)
                                     .DomainSerialization(serializeMessagesCreators)
                                     .RemoteActorProvider()
                                     .Remote(conf.Address)
                                     .SqlPersistence(persistence);
        }

        
    }
}