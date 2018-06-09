using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Tests.Acceptance;
using NBench;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateActor {

    public class AggregateActorPerfSql: AggregateActorPerf
    {
        public AggregateActorPerfSql(ITestOutputHelper output):
            base(output,new StressTestNodeConfiguration().ToStandAloneSystemConfig(AutoTestNodeDbConfiguration.Default).Build().ToString())
        {
            
        }
    }
}