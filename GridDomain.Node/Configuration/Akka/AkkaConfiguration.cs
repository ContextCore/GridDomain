using System.Collections.Generic;
using GridDomain.Node.Configuration.Akka.Hocon;

namespace GridDomain.Node.Configuration.Akka
{
    public class AkkaConfiguration
    {
        public enum LogVerbosity
        {
            Warning,
            Error,
            Info,
            Trace
        }


        private readonly Dictionary<LogVerbosity, string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
        {
            {LogVerbosity.Info, "INFO"},
            {LogVerbosity.Error, "ERROR"},
            {LogVerbosity.Trace, "DEBUG"},
            {LogVerbosity.Warning, "WARNING"}
        };

        private readonly LogVerbosity _logLevel;

        public AkkaConfiguration(IAkkaNetworkAddress networkConf,
            IAkkaDbConfiguration dbConf,
            LogVerbosity logLevel = LogVerbosity.Warning)
        {
            Network = networkConf;
            Persistence = dbConf;
            _logLevel = logLevel;
            LogLevel = _akkaLogLevels[logLevel];
        }

        public string LogLevel { get; }

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

            return new AkkaConfiguration(network, Persistence, _logLevel);
        }

        public string ToClusterSeedNodeSystemConfig(params IAkkaNetworkAddress[] otherSeeds)
        {
            var cfg = new RootConfig(
                new LogConfig(LogLevel, false),
                ClusterConfig.SeedNode(Network, otherSeeds),
                new PersistenceConfig(this));
            return cfg.Build();
        }


        public string ToStandAloneSystemConfig()
        {
            var cfg = new RootConfig(
                new LogConfig(LogLevel, false),
                new StandAloneConfig(Network),
                new PersistenceConfig(this));
            return cfg.Build();
        }

        public string ToStandAloneInMemorySystemConfig()
        {
            var cfg = new RootConfig(
                new LogConfig(LogLevel, true),
                new StandAloneConfig(Network),
                new InMemoryJournalConfig(
                     // new EmptyConfig()
                    new DomainEventAdaptersConfig()
                    ));

            return cfg.Build();
        }

        public string ToClusterNonSeedNodeSystemConfig(params IAkkaNetworkAddress[] seeds)
        {
            var cfg = new RootConfig(
                new LogConfig(LogLevel, false),
                ClusterConfig.NonSeedNode(Network, seeds),
                new PersistenceConfig(this));
            return cfg.Build();
        }
    }
}