using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance
{
    class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public  AutoTestAkkaConfiguration():base(new AutoTestAkkaNetworkAddress(), 
                                                 new AutoTestAkkaDbConfiguration(),
                                                 LogVerbosity.Trace)
        {
            
        }
    }
}