using GridDomain.Node.Configuration;

namespace SchedulerDemo.Configuration
{
    public class LocalAkkaConfiguration : AkkaConfiguration
    {
        public LocalAkkaConfiguration(LogVerbosity verbosity = LogVerbosity.Warning)
            : base(new LocalAkkaNetworkAddress(),
                new LocalAkkaDbConfiguration(),
                verbosity)
        {
        }
    }
}