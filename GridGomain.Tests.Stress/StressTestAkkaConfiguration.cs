using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;

namespace Solomoto.Membership.TransferTool
{
    public class StressTestAkkaConfiguration : AkkaConfiguration
    {
        public StressTestAkkaConfiguration(LogVerbosity verbosity = LogVerbosity.Trace)
            : base(new StressTestAkkaNetworkAddress(),
                new AutoTestAkkaDbConfiguration(),
                verbosity)
        {
        }
    }
}