using Akka.Actor;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration
{
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this NodeConfiguration conf)
        {
            return ActorSystem.Create(conf.Name,
                                      conf.ToStandAloneInMemorySystem()
                                          .Build());
        }

        public static IActorSystemConfigBuilder ToStandAloneInMemorySystem(this NodeConfiguration conf, bool serializeMessagesCreators = false)
        {
            return conf.ConfigureStandAloneInMemorySystem(new ActorSystemConfigBuilder(), serializeMessagesCreators);
        }

        public static IActorSystemConfigBuilder ConfigureStandAloneInMemorySystem(this NodeConfiguration conf, IActorSystemConfigBuilder configBuilder, bool serializeMessagesCreators = false)
        {
            return configBuilder.LocalInMemory(serializeMessagesCreators)
                                .EmitLogLevel(conf.LogLevel)
                                .Remote(conf.Address);
        }
    }
}