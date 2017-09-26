using Akka.Event;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateActor {


    public class AggregateActorPerfInMem : AggregateActorPerf
    {
        public AggregateActorPerfInMem(ITestOutputHelper output):base(output, new StressTestNodeConfiguration(LogLevel.ErrorLevel).ToStandAloneInMemorySystemConfig())
        {
            
        }
    }
}