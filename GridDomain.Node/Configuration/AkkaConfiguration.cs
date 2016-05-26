using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace GridDomain.Node.Configuration
{
    class RootConfig : IAkkaConfig
    {
        private readonly IAkkaConfig[] _parts;

        public RootConfig(params IAkkaConfig[] parts)
        {
            _parts = parts;
        }

        public string Build()
        {
            var configStrings = _parts.Select(p => p.Build()).ToArray();
            var configString = string.Join(Environment.NewLine, configStrings);
            return @"akka {
" + configString + @"
}";
        }
    }


    public class AkkaConfiguration
    {
        public string LogLevel { get; }

        public IAkkaNetworkConfiguration Network { get; }
        public IAkkaDbConfiguration Persistence { get; }

        public AkkaConfiguration(IAkkaNetworkConfiguration networkConf,
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
            var networkConf = Network;
            var network = new AkkaNetworkConfiguration(
                                                       networkConf.Host,
                                                       networkConf.Name,
                                                       newPort);

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
      
        public string ToSingleSystemConfig()
        {
            var cfg = new RootConfig(
                        new LogConfig(this),
                        new ActorConfig(),
                        new ActorDeployConfig(this),
                        new BuildPersistenceConfig(this));
            return cfg.Build();
        }

        public override string ToString()
        {
            return ToSingleSystemConfig();
        }
    }


}