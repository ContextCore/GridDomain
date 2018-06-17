using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Unit;
using NBench;
using Xunit.Abstractions;
using NodeConfigurationExtensions = GridDomain.Node.Persistence.Sql.NodeConfigurationExtensions;

namespace GridDomain.Tests.Stress.AggregateActor
{
    public class AggregateActorPerfSql : AggregateActorPerf
    {
        public AggregateActorPerfSql(ITestOutputHelper output) :
            base(output, ActorSystemConfig()) { }

        private static string ActorSystemConfig()
        {
            var cfg = new StressTestNodeConfiguration();
            return NodeConfigurationExtensions.ToStandAloneSystemConfig(ActorSystemConfigBuilder.New(), AutoTestNodeDbConfiguration.Default, cfg.LogLevel, cfg.Address)
                                              .Build()
                                              .ToString();
        }
    }
}