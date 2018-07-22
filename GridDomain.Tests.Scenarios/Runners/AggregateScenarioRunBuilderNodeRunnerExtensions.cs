using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Cluster.Transport;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Scenarios.Runners {
    public static class AggregateScenarioRunBuilderNodeRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Cluster<TAggregate>(this IAggregateScenarioRunBuilder builder, IDomainConfiguration domainConfig, ILogger log) where TAggregate : class, IAggregate // where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            var nodeConfig = new AutoTestNodeConfiguration();
            using (var clusterInfo = await new ActorSystemConfigBuilder()
                                           .Log(LogEventLevel.Verbose)
                                           .DomainSerialization(true)
                                           .Remote(nodeConfig.Address)
                                           .Cluster("ClusterAggregateScenario")
                                           .AutoSeeds(2)
                                           .Workers(2)
                                           .Build()
                                           .AdditionalInit(s =>
                                                           {
                                                               s.InitDistributedTransport();
                                                               return Task.CompletedTask;
                                                           })
                                           .Create())
            {
                var node = new GridNodeBuilder()
                           .ActorSystem(() => clusterInfo.Cluster.System)
                           .DomainConfigurations(domainConfig)
                           .Transport(s => s.InitDistributedTransport())
                           .Log(log)
                           .Build();

                await node.Start();


                var runner = new AggregateScenarioNodeRunner<TAggregate>(node);

                var run = await runner.Run(builder.Scenario);

                return run;
            }
        }
    }
}