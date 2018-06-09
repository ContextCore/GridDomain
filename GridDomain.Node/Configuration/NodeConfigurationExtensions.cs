using Akka.Actor;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration
{
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this NodeConfiguration conf)
        {
            return ActorSystem.Create(conf.Name, conf.ToStandAloneInMemorySystem().BuildHocon());
        }

        public static ActorSystemBuilder ToStandAloneInMemorySystem(this NodeConfiguration conf, bool serializeMessagesCreators = false)
        {
            return conf.ConfigureStandAloneInMemorySystem(ActorSystemBuilder.New(), serializeMessagesCreators);
        }

        public static ActorSystemBuilder ConfigureStandAloneInMemorySystem(this NodeConfiguration conf, ActorSystemBuilder builder, bool serializeMessagesCreators = false)
        {
            return builder.LocalInMemory(serializeMessagesCreators)
                          .Log(conf.LogLevel)
                          .Remote(conf.Address);
        }
    }
}