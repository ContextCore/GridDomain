using System;
using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration
{
    public class NodeConfiguration
    {
        private readonly Type _logActorType;
        private readonly LogLevel _logLevel;

        public NodeConfiguration(INodeNetworkAddress networkConf, INodeDbConfiguration dbConf, Type logActorType=null, LogLevel logLevel = LogLevel.DebugLevel)
        {
            _logActorType = logActorType ?? typeof(SerilogLoggerActor);
            Network = networkConf;
            Persistence = dbConf;
            _logLevel = logLevel;
        }

        public INodeNetworkAddress Network { get; set; }
        public INodeDbConfiguration Persistence { get; set; }

        public NodeConfiguration Copy(int newPort)
        {
            return Copy(null, newPort);
        }

        public NodeConfiguration Copy(string name = null, int? newPort = null)
        {
            var network = new NodeNetworkAddress(name ?? Network.SystemName, Network.Host, newPort ?? Network.PortNumber);

            return new NodeConfiguration(network, Persistence, _logActorType,_logLevel);
        }

        public string ToClusterSeedNodeSystemConfig(params INodeNetworkAddress[] otherSeeds)
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, _logActorType, false),
                                     ClusterConfig.SeedNode(Network, otherSeeds),
                                     new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                                           new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }

        public virtual string ToStandAloneSystemConfig()
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, _logActorType, false),
                                     new StandAloneConfig(Network),
                                     new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                                           new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }

        public virtual string ToStandAloneInMemorySystemConfig()
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, _logActorType, false),
                                     new StandAloneConfig(Network),
                                     new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                                           new LocalFilesystemSnapshotConfig()));

            return cfg.Build();
        }

        public string ToClusterNonSeedNodeSystemConfig(params INodeNetworkAddress[] seeds)
        {
            var cfg = new RootConfig(new LogConfig(_logLevel, _logActorType, false),
                                     ClusterConfig.NonSeedNode(Network, seeds),
                                     new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig()),
                                                           new PersistenceSnapshotConfig(this)));
            return cfg.Build();
        }
    }
}