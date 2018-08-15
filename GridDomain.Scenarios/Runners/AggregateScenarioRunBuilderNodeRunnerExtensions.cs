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
using GridDomain.Node.Configuration;
using GridDomain.Node.Logging;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Scenarios.Builders;
using Serilog;
using Serilog.Events;

namespace GridDomain.Scenarios.Runners
{


    public static class AggregateScenarioRunBuilderNodeRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Node<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder,
                                                                                     IDomainConfiguration domainConfig,
                                                                                     ILogger logger,
                                                                                     string sqlPersistenceConnectionString = null)
            where TAggregate : class, IAggregate
        {
            var name = "NodeScenario" + typeof(TAggregate).BeautyName();
            var nodes = new List<IExtendedGridDomainNode>();

            var actorSystemConfigBuilder = new ActorSystemConfigBuilder()
                                           .EmitLogLevel(LogEventLevel.Verbose, true)
                                           .DomainSerialization();
            if (sqlPersistenceConnectionString != null)
                actorSystemConfigBuilder = actorSystemConfigBuilder.SqlPersistence(new DefaultNodeDbConfiguration(sqlPersistenceConnectionString));

            var clusterConfig = actorSystemConfigBuilder
                                .Cluster(name ?? "ClusterAggregateScenario" + typeof(TAggregate).BeautyName())
                                .AutoSeeds(1)
                                .Build()
                                .Log(s => logger)
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
                                             });

            using (await clusterConfig.CreateCluster())
            {
                var runner = new AggregateScenarioNodeRunner<TAggregate>(nodes.First());

                var run = await runner.Run(builder.Scenario);

                return run;
            }
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Cluster<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder,
                                                                                        IDomainConfiguration domainConfig,
                                                                                        string sqlPersistenceConnectionString,
                                                                                        Func<LoggerConfiguration> logConfigurationFactory = null,
                                                                                        int autoSeeds = 2,
                                                                                        int workers = 2,
                                                                                        string name = null) where TAggregate : class, IAggregate
        {
            var nodes = new List<IExtendedGridDomainNode>();

            var clusterConfig = new ActorSystemConfigBuilder()
                                .EmitLogLevel(LogEventLevel.Verbose, true)
                                .DomainSerialization()
                                .SqlPersistence(new DefaultNodeDbConfiguration(sqlPersistenceConnectionString))
                                .Cluster(name ?? "ClusterAggregateScenario" + typeof(TAggregate).BeautyName())
                                .AutoSeeds(autoSeeds)
                                .Workers(workers)
                                .Build()
                                .Log(s => (logConfigurationFactory ?? (() => new LoggerConfiguration()))()
                                          .WriteToFile(LogEventLevel.Verbose,
                                                       "GridNodeSystem"
                                                       + s.GetAddress()
                                                          .Port)
                                          .CreateLogger())
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
                                             });


            using (await clusterConfig.CreateCluster())
            {
                var runner = new AggregateScenarioNodeRunner<TAggregate>(nodes.First());

                var run = await runner.Run(builder.Scenario);

                return run;
            }
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Cluster<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder,
                                                                                        IDomainConfiguration domainConfig,
                                                                                        Func<LoggerConfiguration> logConfigurationFactory = null,
                                                                                        int autoSeeds = 2,
                                                                                        int workers = 2,
                                                                                        string name = null) where TAggregate : class, IAggregate
        {
            var nodes = new List<IExtendedGridDomainNode>();

            var clusterConfig = new ActorSystemConfigBuilder()
                                .EmitLogLevel(LogEventLevel.Verbose, true)
                                .DomainSerialization()
                                .Cluster(name ?? "ClusterAggregateScenario" + typeof(TAggregate).BeautyName())
                                .AutoSeeds(autoSeeds)
                                .Workers(workers)
                                .Build()
                                .Log(s => (logConfigurationFactory ?? (() => new LoggerConfiguration()))()
                                          .WriteToFile(LogEventLevel.Verbose,
                                                       "GridNodeSystem"
                                                       + s.GetAddress()
                                                          .Port)
                                          .CreateLogger())
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