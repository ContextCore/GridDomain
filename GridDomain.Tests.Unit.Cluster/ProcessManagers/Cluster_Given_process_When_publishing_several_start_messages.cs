using GridDomain.Tests.Unit.ProcessManagers;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_Given_process_When_publishing_several_start_messages : Given_process_When_publishing_several_start_messages
    {
        public Cluster_Given_process_When_publishing_several_start_messages(ITestOutputHelper helper) :
            base(new SoftwareProgrammingProcessManagerFixture(helper).IgnorePipeCommands()
                                                                     .Clustered()
                                                                     .LogLevel(LogEventLevel.Verbose)
                                                                     .DisableInfrastructureLog()) { }
    }
}