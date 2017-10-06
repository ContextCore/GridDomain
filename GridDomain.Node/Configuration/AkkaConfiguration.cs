using System;
using Akka.Event;
using GridDomain.Node.Actors.Serilog;

namespace GridDomain.Node.Configuration {
    public class AkkaConfiguration
    {
        public Type LogActorType { get; set; } = typeof(SerilogLoggerActor);
        public LogLevel LogLevel { get; set; }
        public INodeNetworkAddress Network { get; set; }

        public AkkaConfiguration(INodeNetworkAddress networkConf, LogLevel logLevel = LogLevel.DebugLevel)
        {
            Network = networkConf;
            LogLevel = logLevel;
        }

    }
}