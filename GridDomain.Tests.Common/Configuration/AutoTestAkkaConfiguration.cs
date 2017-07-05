using System.Configuration;
using Akka.Event;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration(LogLevel verbosity = LogLevel.DebugLevel)
            : base(new AutoTestAkkaNetworkAddress(), GetConfig(), verbosity, typeof(LoggerActorDummy)) {}

        private static IAkkaDbConfiguration GetConfig()
        {
            var section = (WriteDbConfigSection)ConfigurationManager.GetSection("WriteDb");

            return section.ElementInformation.IsPresent ? (IAkkaDbConfiguration)section: new AutoTestAkkaDbConfiguration();
        }
    }
}