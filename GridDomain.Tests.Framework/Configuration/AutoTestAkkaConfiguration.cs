using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration(LogVerbosity verbosity = LogVerbosity.Warning)
            : base(new AutoTestAkkaNetworkAddress(),
                new AutoTestAkkaDbConfiguration(),
                verbosity)
        {
        }
    }
}