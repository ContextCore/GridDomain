using System;
using Akka.Event;
using GridDomain.Node.Configuration.Akka.Hocon;

namespace GridDomain.Node.Configuration.Akka
{
    public class AkkaConfiguration
    {
        private readonly Type _logActorType;
        private readonly LogLevel _logLevel;

        public AkkaConfiguration(IAkkaNetworkAddress networkConf,
                                 IAkkaDbConfiguration dbConf,
                                 LogLevel logLevel = LogLevel.DebugLevel,
                                 Type logActorType = null)
        {
            _logActorType = logActorType;
            Network = networkConf;
            Persistence = dbConf;
            _logLevel = logLevel;
        }

        public IAkkaNetworkAddress Network { get; set; }
        public IAkkaDbConfiguration Persistence { get; set; }

        public AkkaConfiguration Copy(int newPort)
        {
            return Copy(null, newPort);
        }

        public AkkaConfiguration Copy(string name = null, int? newPort = null)
        {
            var network = new AkkaNetworkAddress(name ?? Network.SystemName, Network.Host, newPort ?? Network.PortNumber);

            return new AkkaConfiguration(network, Persistence, _logLevel);
        }

        public string ToClusterSeedNodeSystemConfig(params IAkkaNetworkAddress[] otherSeeds)
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, false, _logActorType),
                                     ClusterConfig.SeedNode(Network, otherSeeds),
                                     new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                                           new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }

        public virtual string ToStandAloneSystemConfig()
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, false, _logActorType),
                                     new StandAloneConfig(Network),
                                     new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                                           new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }

        public virtual string ToStandAloneInMemorySystemConfig()
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, false, _logActorType),
                                     new StandAloneConfig(Network),
                                     new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                                           new LocalFilesystemSnapshotConfig()));

            return cfg.Build();
        }

        public string ToClusterNonSeedNodeSystemConfig(params IAkkaNetworkAddress[] seeds)
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, false, _logActorType),
                                     ClusterConfig.NonSeedNode(Network, seeds),
                                     new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                                           new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }
    }
}