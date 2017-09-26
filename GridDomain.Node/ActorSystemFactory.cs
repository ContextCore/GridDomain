using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    public class ActorSystemFactory
    {
        public static AkkaCluster CreateCluster(NodeConfiguration nodeConf, int seedNodeNumber = 2, int childNodeNumber = 3)
        {
            var port = nodeConf.Network.PortNumber;
            var seedNodeConfigs = Enumerable.Range(0, seedNodeNumber)
                                            .Select(n => Copy(nodeConf, port++))
                                            .ToArray();
            var seedAdresses = seedNodeConfigs.Select(s => s.Network)
                                              .ToArray();

            var seedSystems =
                seedNodeConfigs.Select(
                                       c => ActorSystem.Create(c.Network.SystemName, c.ToClusterSeedNodeSystemConfig()));

            var nonSeedConfiguration =
                Enumerable.Range(0, childNodeNumber)
                          .Select(
                                  n =>
                                      ActorSystem.Create(nodeConf.Network.SystemName,
                                                         nodeConf.ToClusterNonSeedNodeSystemConfig(seedAdresses)));


            return new AkkaCluster {SeedNodes = seedSystems.ToArray(), NonSeedNodes = nonSeedConfiguration.ToArray()};
        }

        private static NodeConfiguration Copy(NodeConfiguration cfg, int newPort)
        {
            return new NodeConfiguration(new NodeNetworkAddress(cfg.Network.SystemName, cfg.Network.Host, newPort),
                                         cfg.Persistence,
                                         cfg.LogLevel){LogActorType = cfg.LogActorType };
        }
    }
}