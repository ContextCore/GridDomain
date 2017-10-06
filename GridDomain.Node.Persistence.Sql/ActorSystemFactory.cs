using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Node
{
    public class ActorSystemFactory
    {
        public static AkkaCluster CreateCluster(AkkaConfiguration akkaConf, int seedNodeNumber = 2, int childNodeNumber = 3)
        {
            var port = akkaConf.Network.PortNumber;
            var seedNodeConfigs = Enumerable.Range(0, seedNodeNumber)
                                            .Select(n => Copy(akkaConf, port++))
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
                                      ActorSystem.Create(akkaConf.Network.SystemName,
                                                         akkaConf.ToClusterNonSeedNodeSystemConfig(seedAdresses)));


            return new AkkaCluster {SeedNodes = seedSystems.ToArray(), NonSeedNodes = nonSeedConfiguration.ToArray()};
        }

        private static AkkaConfiguration Copy(AkkaConfiguration cfg, int newPort)
        {
            return new AkkaConfiguration(new NodeNetworkAddress(cfg.Network.SystemName, cfg.Network.Host, newPort),
                                         cfg.Persistence,
                                         cfg.LogLevel){LogActorType = cfg.LogActorType };
        }
    }
}