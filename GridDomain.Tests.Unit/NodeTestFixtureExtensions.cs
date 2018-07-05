using System;
using Akka.TestKit.TestActors;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Actors.CommandPipe.Messages;
using Akka.Actor;
using Akka.Configuration;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;
using GridDomain.Node.Logging;
using GridDomain.ProcessManagers.State;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using Serilog.Events;
using Serilog.Filters;

namespace GridDomain.Tests.Unit
{
    public static class NodeTestFixtureExtensions
    {
        public static NodeTestFixture EnableScheduling(this NodeTestFixture fixture, IRetrySettings config)
        {
            return EnableScheduling(fixture, new InMemoryQuartzConfig(config));
        }

        public static T LogLevel<T>(this T fixture, LogEventLevel value) where T : NodeTestFixture
        {
            fixture.NodeConfig.LogLevel = value;
            fixture.ActorSystemConfigBuilder.Log(value);
            return fixture;
        }
        public static T Log<T>(this T fixture, LogConfig value) where T : NodeTestFixture
        {
            fixture.NodeConfig.LogLevel = value.LogEventLevel;
            fixture.ActorSystemConfigBuilder.Add(value);
            return fixture;
        }

        public static T DisableInfrastructureLog<T>(this T fixture) where T : NodeTestFixture
        {
            fixture.LoggerConfiguration
                   .Filter.ByExcluding(Matching.WithProperty<string>(LogContextNames.Class, s => s.StartsWith("Akka.")));

            return fixture;
        }

        public static NodeTestFixture EnableScheduling(this NodeTestFixture fixture, IQuartzConfig config = null, bool clearScheduledData = true)
        {
            IQuartzConfig quartzConfig = config ?? new InMemoryQuartzConfig(new InMemoryRetrySettings(5, TimeSpan.FromMinutes(10), new DefaultExceptionPolicy()));
            fixture.DomainConfigurations.Add(new FutureAggregateHandlersDomainConfiguration());

            fixture.OnNodeCreatedEvent += (o, node) =>
                                          {
                                              node.System.InitSchedulingExtension(quartzConfig,
                                                                                  node.Log,
                                                                                  node.Transport,
                                                                                  node);
                                          };
            if (clearScheduledData)
                fixture.OnNodeStartedEvent += (sender, args) => fixture.Node.System.GetExtension<SchedulingExtension>()
                                                                       .Scheduler.Clear();

            return fixture;
        }

        public static NodeTestFixture UseAdaper<TFrom, TTo>(this NodeTestFixture fixture, ObjectAdapter<TFrom, TTo> adapter)
        {
            fixture.OnNodeCreatedEvent += (sender, node) => node.EventsAdaptersCatalog.Register(adapter);
            return fixture;
        }

        public static NodeTestFixture UseAdaper<TFrom, TTo>(this NodeTestFixture fixture, DomainEventAdapter<TFrom, TTo> adapter) where TFrom : DomainEvent
                                                                                                                                  where TTo : DomainEvent
        {
            fixture.OnNodeCreatedEvent += (sender, node) => node.EventsAdaptersCatalog.Register(adapter);
            return fixture;
        }

        public static T PrintSystemConfig<T>(this T fixture)where T:NodeTestFixture
        {
            fixture.OnNodePreparingEvent += (s, e) => e.NodeBuilder.Logger.Warning(fixture.ActorSystemConfigBuilder.Build().ToString());
            return fixture;
        }

        public static NodeTestFixture IgnorePipeCommands(this NodeTestFixture fixture)
        {
            fixture.OnNodeCreatedEvent += (sender, node) => { };

            fixture.OnNodeStartedEvent += (sender, e) =>
                                          {
                                              //cannot work for cluster mode 
                                              //supress errors raised by commands not reaching aggregates
                                              //var nullActor = fixture.Node.System.ActorOf(BlackHoleActor.Props);
                                              //fixture.Node.Pipe.ProcessesPipeActor.Ask<Initialized>(new Initialize(nullActor))
                                              //       .Wait();
                                          };

            return fixture;
        }
    }
}