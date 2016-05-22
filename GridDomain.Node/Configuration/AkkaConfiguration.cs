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
    public class AkkaConfiguration
    {
        public int Port { get; }
        public string Name { get; }
        public string Host { get; }

        public string JournalConnectionString { get; }
        public string SnapshotConnectionString { get; }
        public string LogLevel { get; }


        
        public AkkaConfiguration(IAkkaNetworkConfiguration networkConf,
                                 IAkkaDbConfiguration dbConf,
                                 LogVerbosity logLevel = LogVerbosity.Warning
                                )
        {
            _networkConf = networkConf;
            _dbConf = dbConf;
            _logLevel = logLevel;
            Name = networkConf.Name;
            Port = networkConf.PortNumber;
            Host = networkConf.Host;
            LogLevel = _akkaLogLevels[logLevel];
            JournalConnectionString = dbConf.JournalConnectionString;
            SnapshotConnectionString = dbConf.SnapshotConnectionString;
        }

        public AkkaConfiguration Copy(int newPort)
        {
            return new AkkaConfiguration(_networkConf, _dbConf, _logLevel);

        }

        private readonly Dictionary<LogVerbosity,string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
                                                            {
                                                                {LogVerbosity.Info, "INFO"},
                                                                {LogVerbosity.Error, "ERROR"},
                                                                {LogVerbosity.Trace, "DEBUG"},
                                                                {LogVerbosity.Warning, "WARNING"}
                                                            };

        private readonly LogVerbosity _logLevel;
        private IAkkaDbConfiguration _dbConf;
        private readonly IAkkaNetworkConfiguration _networkConf;

        public enum LogVerbosity
        {
            Warning,
            Error,
            Info,
            Trace
        }
    }


}