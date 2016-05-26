using System.Collections.Generic;
using System.Data.Common;

namespace GridDomain.Node.Configuration
{
    public interface IAkkaNetworkConfiguration
    {
        string Name { get; }
        string Host { get; }
        int PortNumber { get; }
    }


    public class AkkaNetworkConfiguration : IAkkaNetworkConfiguration
    {
        public string Name { get;}
        public string Host { get;}
        public int PortNumber { get; }

        public AkkaNetworkConfiguration(string name, string host, int port )
        {
            Name = name;
            Host = host;
            PortNumber = port;
        }
    }

    public class AkkaConfiguration
    {
        public int Port => Network.PortNumber;
        public string Name => Network.Name;
        public string Host => Network.Host;

        public string JournalConnectionString => Persistence.JournalConnectionString;
        public string SnapshotConnectionString => Persistence.SnapshotConnectionString;
        public string LogLevel { get; }
        public string MetadataTableName => Persistence.MetadataTableName;
        public string JournalTableName => Persistence.JournalTableName;
        public string SnapshotTableName => Persistence.SnapshotTableName;

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