using Akka.Actor;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Node
{
    public static class AkkaConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this AkkaConfiguration conf)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneInMemorySystemConfig());
        }

        public static ActorSystem CreateSystem(this AkkaConfiguration conf)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneSystemConfig());
        }
    }
}