using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Node
{
    public class ActorSystemFactory
    {
        public static AkkaCluster CreateCluster(AkkaConfiguration akkaConf, int seedNodeNumber = 2,
            int childNodeNumber = 3)
        {
            var port = akkaConf.Network.PortNumber;
            var seedNodeConfigs = Enumerable.Range(0, seedNodeNumber).Select(n => akkaConf.Copy(port++)).ToArray();
            var seedAdresses = seedNodeConfigs.Select(s => s.Network).ToArray();

            var seedSystems =
                seedNodeConfigs.Select(
                    c => ActorSystem.Create(c.Network.SystemName, c.ToClusterSeedNodeSystemConfig(seedAdresses)));

            var nonSeedConfiguration = Enumerable.Range(0, childNodeNumber)
                .Select(
                    n =>
                        ActorSystem.Create(akkaConf.Network.SystemName,
                            akkaConf.ToClusterNonSeedNodeSystemConfig(seedAdresses)));


            return new AkkaCluster {SeedNodes = seedSystems.ToArray(), NonSeedNodes = nonSeedConfiguration.ToArray()};
        }

        [Obsolete("Use extensions methods fot AkkaConfiguration from AkkaConfigurationExtensions instead")]
        public static ActorSystem CreateActorSystem(AkkaConfiguration akkaConf)
        {
            return akkaConf.CreateSystem();
        }
    }
}