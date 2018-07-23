using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Cluster.Transport;
using GridDomain.Node.Configuration;
using GridDomain.Node.Logging;
using GridDomain.Tests.Common.Configuration;
using Serilog;
using Serilog.Events;

namespace GridDomain.Tests.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderNodeRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> ClusterNode<TAggregate>(this IAggregateScenarioRunBuilder builder,
                                                                                        IDomainConfiguration domainConfig,
                                                                                        Func<LoggerConfiguration> logConfigurtionFactory
                                                                                        ) where TAggregate : class, IAggregate
        {
            return await Cluster<TAggregate>(builder, domainConfig, logConfigurtionFactory, 1, 0,"ClusterNodeAggregateScenario_"+typeof(TAggregate).BeautyName());
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Cluster<TAggregate>(this IAggregateScenarioRunBuilder builder, 
                                                                                        IDomainConfiguration domainConfig, 
                                                                                        Func<LoggerConfiguration> logConfigurationFactory,
                                                                                        int autoSeeds = 2, int workers = 2, string name = null) where TAggregate : class, IAggregate // where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            var clusterConfig = new ActorSystemConfigBuilder()
                                .EmitLogLevel(LogEventLevel.Verbose)
                                .DomainSerialization()
                                .Cluster(name ?? "ClusterAggregateScenario"+typeof(TAggregate).BeautyName())
                                .AutoSeeds(autoSeeds)
                                .Workers(workers)
                                .Build();
            var nodes = new List<IExtendedGridDomainNode>();
            using (var clusterInfo = await clusterConfig
                                           .Log(s =>
                                                
                                                    logConfigurationFactory().WriteToFile(LogEventLevel.Verbose,"GridNodeSystem"+s.GetAddress().Port)
                                                                             .CreateLogger()
                                                )
                                           .OnClusterUp(async s =>
                                                        {
                                                               var ext = s.GetExtension<LoggingExtension>();

                                                               var node = new ClusterNodeBuilder()
                                                                   .ActorSystem(() => s)
                                                                   .DomainConfigurations(domainConfig)
                                                                   .Log(ext.Logger)
                                                                   .Build();

                                                               nodes.Add(node);
                                                               await node.Start();
                                                           })
                                           .Create())
            {

                var runner = new AggregateScenarioNodeRunner<TAggregate>(nodes.First());

                var run = await runner.Run(builder.Scenario);

                return run;
            }
        }
    }
}