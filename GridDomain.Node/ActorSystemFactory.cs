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
        public static ActorSystem[] CreateCluster(AkkaConfiguration akkaConf, int seedNodeNumber,int childNodeNumber)
        {
            var port = akkaConf.Network.PortNumber;
            var nameNumber = 0;
            var seedNodeConfigs = Enumerable.Range(0, seedNodeNumber).Select(n => akkaConf.Copy(port++)).ToArray();
            var seedAdresses = seedNodeConfigs.Select(s => s.Network).ToArray();


            var seedSystems= seedNodeConfigs.Select(c => ActorSystem.Create(c.Network.Name, c.ToClusterSeedNodeSystemConfig(seedAdresses)));

            var nonSeedConfiguration = Enumerable.Range(0, childNodeNumber)
                                                 .Select(n => ActorSystem.Create(akkaConf.Network.Name + nameNumber++, akkaConf.ToClusterNonSeedNodeSystemConfig(seedAdresses)));

            return seedSystems.Concat(nonSeedConfiguration).ToArray();
        }

        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf)
        {
            var actorSystem = ActorSystem.Create(akkaConf.Network.Name, akkaConf.ToClusterSeedNodeSystemConfig());
            return actorSystem;
        }
    }
}