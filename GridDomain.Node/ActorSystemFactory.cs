using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Akka.Persistence.SqlServer;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    public class ActorSystemFactory
    {
        public static ActorSystem[] CreateCluster(AkkaConfiguration akkaConf, int seedNodeNumber=2,int childNodeNumber=3)
        {
            var port = akkaConf.Network.PortNumber;
            var seedNodeConfigs = Enumerable.Range(0, seedNodeNumber).Select(n => akkaConf.Copy(port++)).ToArray();
            var seedAdresses = seedNodeConfigs.Select(s => s.Network).ToArray();

            var seedSystems= seedNodeConfigs.Select(c => ActorSystem.Create(c.Network.SystemName, c.ToClusterSeedNodeSystemConfig(seedAdresses)));

            var nonSeedConfiguration = Enumerable.Range(0, childNodeNumber)
                                                 .Select(n => ActorSystem.Create(akkaConf.Network.SystemName , akkaConf.ToClusterNonSeedNodeSystemConfig(seedAdresses)));

            return seedSystems.Concat(nonSeedConfiguration).ToArray();
        }

        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf)
        {
            var standAloneSystemConfig = akkaConf.ToStandAloneSystemConfig();
            var actorSystem = ActorSystem.Create(akkaConf.Network.SystemName, standAloneSystemConfig);
            return actorSystem;
        }
    }
}