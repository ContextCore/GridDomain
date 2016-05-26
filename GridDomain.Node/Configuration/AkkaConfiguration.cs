using System.Collections.Generic;
using System.Data.Common;

namespace GridDomain.Node.Configuration
{
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

        private readonly Dictionary<LogVerbosity,string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
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
    }


}