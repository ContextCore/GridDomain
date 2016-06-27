using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

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