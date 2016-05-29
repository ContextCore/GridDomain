using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance
{
    class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public  AutoTestAkkaConfiguration(LogVerbosity verbosity = LogVerbosity.Warning) :base(new AutoTestAkkaNetworkAddress(), 
                                                 new AutoTestAkkaDbConfiguration(),
                                                 verbosity)
        {
            
        }
    }
}