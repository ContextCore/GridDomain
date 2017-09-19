using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS;
using NBench;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateActor {

    public class AggregateActorPerfSql: AggregateActorPerf
    {
        public AggregateActorPerfSql(ITestOutputHelper output):base(output,
                                                                           new StressTestAkkaConfiguration(LogLevel.ErrorLevel).ToStandAloneSystemConfig())
        {
            
        }
    }
}