using System;
using System.Configuration;
using System.Reflection;
using Akka.Event;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Acceptance.Tools {
    public class AcceptanceAutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AcceptanceAutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestAkkaNetworkAddress(), new AutoTestAkkaDbConfiguration(), verbosity, typeof(LoggerActorDummy)) { }
    }
}