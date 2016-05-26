using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance
{
    class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public  AutoTestAkkaConfiguration():base(new AutoTestAkkaNetworkConfiguration(), 
            new AutoTestAkkaDbConfiguration(),
            LogVerbosity.Warning)
        {
            
        }
    }
}