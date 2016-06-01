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

        public AkkaConfiguration Copy(int newPort)
        {
            return Copy(null,newPort);
        }

        public AkkaConfiguration Copy(string name = null, int? newPort = null)
        {
            var network = new AkkaNetworkAddress(name ?? Network.SystemName,
                                                 Network.Host,
                                                 newPort ?? Network.PortNumber);

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

        public string ToClusterSeedNodeSystemConfig(params IAkkaNetworkAddress[] otherSeeds)
        {
            var cfg = new RootConfig(
                        new LogConfig(this,false),
                        ClusterConfig.SeedNode(Network,otherSeeds),
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