using System.Collections.Generic;

namespace GridDomain.Node.Configuration
{
    public class AkkaConfiguration
    {
        public int Port { get; }
        public string Name { get; }
        public string Host { get; }

        public string LogLevel { get; }
        public AkkaConfiguration(string name, int portNumber, string host, LogVerbosity logLevel = LogVerbosity.Warning)
        {
            Name = name;
            Port = portNumber;
            Host = host;
            LogLevel = _akkaLogLevels[logLevel];
        }

        private readonly Dictionary<LogVerbosity,string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
                                                            {
                                                                {LogVerbosity.Info, "INFO"},
                                                                {LogVerbosity.Error, "ERROR"},
                                                                {LogVerbosity.Trace, "DEBUG"},
                                                                {LogVerbosity.Warning, "WARNING"}
                                                            }; 
        public enum LogVerbosity
        {
            Warning,
            Error,
            Info,
            Trace
        }
    }


}