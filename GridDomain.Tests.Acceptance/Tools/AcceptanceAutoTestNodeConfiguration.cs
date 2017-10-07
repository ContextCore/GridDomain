using System;
using System.Configuration;
using System.Reflection;
using Akka.Event;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using Serilog.Events;

namespace GridDomain.Tests.Acceptance.Tools {
    public class AcceptanceAutoTestNodeConfiguration : NodeConfiguration
    {
        public AcceptanceAutoTestNodeConfiguration(LogEventLevel verbosity = LogEventLevel.Verbose)
            : base("TestSystem",new AutoTestNodeNetworkAddress(), verbosity)
        {
            LogActorType = typeof(ConsoleSerilogLoggerActor);
        }
    }
}