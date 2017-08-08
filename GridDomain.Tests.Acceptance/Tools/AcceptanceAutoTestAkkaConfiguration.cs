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
            : base(new AutoTestAkkaNetworkAddress(), GetConfig(), verbosity, typeof(LoggerActorDummy)) { }

        private static IAkkaDbConfiguration GetConfig()
        {
            var config = ConfigurationManager.OpenExeConfiguration(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var section = (WriteDbConfigSection)config.GetSection("WriteDb");
            return section?.ElementInformation.IsPresent == true ? (IAkkaDbConfiguration)section : new AutoTestAkkaDbConfiguration();
        }
    }
}