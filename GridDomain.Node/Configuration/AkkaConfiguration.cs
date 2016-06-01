using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace GridDomain.Node.Configuration
{
    public class AkkaConfiguration
    {
        public string LogLevel { get; }

        public IAkkaNetworkAddress Network { get; }
        public IAkkaDbConfiguration Persistence { get; }

        public AkkaConfiguration(IAkkaNetworkAddress networkConf,
                                 IAkkaDbConfiguration dbConf,
                                 LogVerbosity logLevel = LogVerbosity.Warning)
        {
            Network = networkConf;
            Persistence = dbConf;
            _logLevel = logLevel;
            LogLevel = _akkaLogLevels[logLevel];
        }

        public AkkaConfiguration Copy(int? newPort = null)
        {
            var networkConf = Network;
            var network = new AkkaNetworkAddress(networkConf.SystemName,
                                                 networkConf.Host,
                                                 newPort ?? networkConf.PortNumber);

            return new AkkaConfiguration(network, Persistence, _logLevel);
        }

        private readonly Dictionary<LogVerbosity, string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
                                                                           {
                                                                               {LogVerbosity.Info, "INFO"},
                                                                               {LogVerbosity.Error, "ERROR"},
                                                                               {LogVerbosity.Trace, "DEBUG"},
                                                                               {LogVerbosity.Warning, "WARNING"}
                                                                           };
        private readonly LogVerbosity _logLevel;

        public enum LogVerbosity
        {
            Warning,
            Error,
            Info,
            Trace
        }

        public string ToClusterSeedNodeSystemConfig(string name = null, params IAkkaNetworkAddress[] otherSeeds)
        {
            var akkaNetworkAddress = new AkkaNetworkAddress(name ?? Network.SystemName,Network.Host,Network.PortNumber);
            var cfg = new RootConfig(
                        new LogConfig(this,false),
                        ClusterConfig.SeedNode(akkaNetworkAddress,otherSeeds),
                        new PersistenceConfig(this));
            return cfg.Build();
        }

        public string ToStandAloneSystemConfig()
        {
            var cfg = new RootConfig(
                        new LogConfig(this),
                        new StandAloneConfig(Network),
                        new PersistenceConfig(this));
            return cfg.Build();
        }


        public string ToClusterNonSeedNodeSystemConfig(params IAkkaNetworkAddress[] seeds)
        {
            var cfg = new RootConfig(
                        new LogConfig(this, false),
                        ClusterConfig.NonSeedNode(Network, seeds),
                        new PersistenceConfig(this));
            return cfg.Build();
        }
    }


}