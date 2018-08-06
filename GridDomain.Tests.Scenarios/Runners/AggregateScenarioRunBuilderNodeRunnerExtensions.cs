using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.DI.AutoFac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Logging;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderNodeRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Node<TAggregate>(this IAggregateScenarioRunBuilder builder,
                                                                                            IDomainConfiguration domainConfig,
                                                                                            Func<LoggerConfiguration> logConfigurtionFactory
        ) where TAggregate : class, IAggregate
        {
            return await TestCluster<TAggregate>(builder, domainConfig, logConfigurtionFactory, 1, 0, "NodeScenario" + typeof(TAggregate).BeautyName());
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> TestCluster<TAggregate>(this IAggregateScenarioRunBuilder builder,
                                                                                        IDomainConfiguration domainConfig,
                                                                                        Func<LoggerConfiguration> logConfigurationFactory,
                                                                                        int autoSeeds = 1,
                                                                                        int workers = 1,
                                                                                        string name = null) where TAggregate : class, IAggregate
        {
            var nodes = new List<IExtendedGridDomainNode>();

            var clusterConfig = new ActorSystemConfigBuilder()
                                .EmitLogLevel(LogEventLevel.Verbose,true)
                                .DomainSerialization()
                                .Cluster(name ?? "ClusterAggregateScenario" + typeof(TAggregate).BeautyName())
                                .AutoSeeds(autoSeeds)
                                .Workers(workers)
                                .Build()
                                .Log(s => logConfigurationFactory()
                                             .WriteToFile(LogEventLevel.Verbose,"GridNodeSystem"+ s.GetAddress().Port)
                                             .CreateLogger())

                                .OnClusterUp(async s=>
                                                {
                                                 var ext = s.GetExtension<LoggingExtension>();
                                                 var node = new ClusterNodeBuilder()
                                                            .ActorSystem(() => s)
                                                            .DomainConfigurations(domainConfig)
                                                            .Log(ext.Logger)
                                                            .Build();

                                                 nodes.Add(node);
                                                 await node.Start();
                                             });


            using (await clusterConfig.CreateCluster())
            {
                var runner = new AggregateScenarioClusterInMemoryRunner<TAggregate>(nodes.ToArray());

                var run = await runner.Run(builder.Scenario);

                return run;
            }
        }
    }
}