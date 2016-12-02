using System.Collections.Generic;
using GridDomain.Node.Configuration.Akka.Hocon;

namespace GridDomain.Node.Configuration.Akka
{
    public enum LogVerbosity
    {
        Warning,
        Error,
        Info,
        Trace
    }
    public class AkkaConfiguration
    {
        private readonly LogVerbosity _logVerbosity;

        public AkkaConfiguration(IAkkaNetworkAddress networkConf,
            IAkkaDbConfiguration dbConf,
            LogVerbosity logVerbosity = LogVerbosity.Warning)
        {
            Network = networkConf;
            Persistence = dbConf;
            _logVerbosity = logVerbosity;
        }

        public IAkkaNetworkAddress Network { get; }
        public IAkkaDbConfiguration Persistence { get; }

        public AkkaConfiguration Copy(int newPort)
        {
            return Copy(null, newPort);
        }

        public AkkaConfiguration Copy(string name = null, int? newPort = null)
        {
            var network = new AkkaNetworkAddress(name ?? Network.SystemName,
                Network.Host,
                newPort ?? Network.PortNumber);

            return new AkkaConfiguration(network, Persistence, _logVerbosity);
        }

        public string ToClusterSeedNodeSystemConfig(params IAkkaNetworkAddress[] otherSeeds)
        {
            var cfg = new RootConfig(
                new LogConfig(_logVerbosity,false),
                ClusterConfig.SeedNode(Network, otherSeeds),
                new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                      new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }


        public string ToStandAloneSystemConfig()
        {
            var cfg = new RootConfig(
                new LogConfig(_logVerbosity,false),
                new StandAloneConfig(Network),
                new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                    new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }

        public string ToStandAloneInMemorySystemConfig()
        {
            var cfg = new RootConfig(
                new LogConfig(_logVerbosity,false),
                new StandAloneConfig(Network),
                new PersistenceConfig(new InMemoryJournalConfig(
                                                    new DomainEventAdaptersConfig()),
                                       new LocalFilesystemSnapshotConfig())
                                    );

            return cfg.Build();
        }

        public string ToClusterNonSeedNodeSystemConfig(params IAkkaNetworkAddress[] seeds)
        {
            var cfg = new RootConfig(
                new LogConfig(_logVerbosity,false),
                ClusterConfig.NonSeedNode(Network, seeds),
                new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                               new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }
    }
}