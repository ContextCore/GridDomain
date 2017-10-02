using System;
using Akka.Event;
using GridDomain.Node.Actors.Serilog;

namespace GridDomain.Node.Configuration
{
    public class NodeConfiguration
    {
        public Type LogActorType { get; set; } = typeof(SerilogLoggerActor);
        public LogLevel LogLevel { get; set; }

        public NodeConfiguration(INodeNetworkAddress networkConf, INodeDbConfiguration dbConf, LogLevel logLevel = LogLevel.DebugLevel)
        {
            Network = networkConf;
            Persistence = dbConf;
            LogLevel = logLevel;
        }
        public NodeConfiguration(INodeNetworkAddress networkConf, LogLevel logLevel = LogLevel.DebugLevel):this(networkConf,null,logLevel)
        {
        }

        public INodeNetworkAddress Network { get; set; }
        public INodeDbConfiguration Persistence { get; set; }
    }
}