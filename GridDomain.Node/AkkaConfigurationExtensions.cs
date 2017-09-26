using Akka.Actor;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    public static class AkkaConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this NodeConfiguration conf)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneInMemorySystemConfig());
        }

        public static ActorSystem CreateSystem(this NodeConfiguration conf)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneSystemConfig());
        }
    }
}