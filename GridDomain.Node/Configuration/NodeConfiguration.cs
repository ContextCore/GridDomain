using System;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    public class NodeConfiguration
    {
        public LogEventLevel LogLevel { get; set; }
        public INodeNetworkAddress Address { get; set; }
        public string Name { get; }

        public NodeConfiguration(string name, INodeNetworkAddress addressConf, LogEventLevel logLevel = LogEventLevel.Verbose)
        {
            Address = addressConf;
            LogLevel = logLevel;
            Name = name;
        }

    }
}