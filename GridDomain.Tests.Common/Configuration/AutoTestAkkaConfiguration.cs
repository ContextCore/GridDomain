using System;
using System.Configuration;
using System.Reflection;
using Akka.Event;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestAkkaNetworkAddress(), new AutoTestAkkaDbConfiguration(), verbosity, typeof(LoggerActorDummy)) {}
    }
}