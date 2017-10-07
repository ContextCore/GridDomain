using System;
using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    public class NodeConfiguration
    {
        public Type LogActorType { get; set; } = typeof(SerilogLoggerActor);
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