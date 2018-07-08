using System;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    public class NodeConfiguration
    {
        public LogEventLevel LogLevel { get; set; }
        public INodeNetworkAddress Address { get; }
        public string Name { get;}

        public NodeConfiguration(string name, INodeNetworkAddress addressConf, LogEventLevel logLevel = LogEventLevel.Information)
        {
            Address = addressConf;
            LogLevel = logLevel;
            Name = name;
        }
    }

    public static class NodeNetworkAddressExtensions
    {
        public static string ToFullTcpAddress(this INodeNetworkAddress conf, string name)
        {
            return $"akka.tcp://{name}@{conf.Host}:{conf.PortNumber}";
        }
    }
}