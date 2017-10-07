using Akka.Event;
using GridDomain.Node.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateActor {


    public class AggregateActorPerfInMem : AggregateActorPerf
    {
        public AggregateActorPerfInMem(ITestOutputHelper output):base(output, 
            new StressTestNodeConfiguration().ToStandAloneInMemorySystemConfig())
        {
            
        }
    }
}