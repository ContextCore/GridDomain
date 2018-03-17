using System.Linq;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Cluster
{
    public class ActorSystemFactory
    {
        public static AkkaCluster CreateCluster(NodeConfiguration nodeConf, int seedNodeNumber = 2, int childNodeNumber = 3)
        {
            var port = nodeConf.Address.PortNumber;
            var seedNodeConfigs = Enumerable.Range(0, seedNodeNumber)
                                            .Select(n => Copy(nodeConf, port++))
                                            .ToArray();

            var seedAdresses = seedNodeConfigs.Select(s => s.Address)
                                              .ToArray();

            //var seedSystems =
            //    seedNodeConfigs.Select(
            //                           c => ActorSystem.Create(c.Network.SystemName, c.ToClusterSeedNodeSystemConfig()));
            //
            //var nonSeedConfiguration =
            //    Enumerable.Range(0, childNodeNumber)
            //              .Select(
            //                      n =>
            //                          ActorSystem.Create(akkaConf.Network.SystemName,
            //                                             akkaConf.ToClusterNonSeedNodeSystemConfig(seedAdresses)));
            //
            //
            // return new AkkaCluster {SeedNodes = seedSystems.ToArray(), NonSeedNodes = nonSeedConfiguration.ToArray()};
            return null;
        }

        private static NodeConfiguration Copy(NodeConfiguration cfg, int newPort)
        {
            return new NodeConfiguration(cfg.Name,
                                         new NodeNetworkAddress(cfg.Address.Host, newPort),
                                         cfg.LogLevel);
        }
    }
}