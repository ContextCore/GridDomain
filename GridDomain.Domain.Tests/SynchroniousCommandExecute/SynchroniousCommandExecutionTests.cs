using System;
using System.Diagnostics;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.SyncProjection.SampleDomain;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    class SynchroniousCommandExecutionTests : NodeCommandsTest
    {

        public SynchroniousCommandExecutionTests():base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "SyncExecution", false)
        {
        }

        protected override TimeSpan Timeout => Debugger.IsAttached
            ? TimeSpan.FromMinutes(10)
            : TimeSpan.FromSeconds(5);

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();

            var config = new CustomContainerConfiguration(
                     c => c.RegisterAggregate<SampleAggregate, TestAggregatesCommandHandler>(),
                     c => c.RegisterInstance(new InMemoryQuartzConfig()),
                     c => c.RegisterType<AggregateCreatedProjectionBuilder>());

            container.Register(config);

            var actorAllSystems = ActorSystem.Create("test",akkaConf.ToStandAloneInMemorySystemConfig());
            return new GridDomainNode(config,
                                      new TestRouteMap(new UnityServiceLocator(container)),
                                      TransportMode.Standalone, actorAllSystems);
        }

       
        [Then]
        public void When_command_executed_synchroniosly_Then_aggregate_already_has_events_after_finish()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand,
                Timeout,
                ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                                                            syncCommand.AggregateId)
                );
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Then]
        public void Can_synchroniosly_execute_commands_ask_Node_to_wait_for_notification_events()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand,
                            Timeout,
                            ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                    syncCommand.AggregateId)
                            );
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Then]
        public void Can_synchroniosly_execute_commands_waiting_for_notification_events_bySelf()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            var task = GridNode.Execute<AggregateChangedEventNotification>
                                        (syncCommand,
                                         ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,
                                                                                                syncCommand.AggregateId)
                                        );
            if(!task.Wait(Timeout))
                throw new TimeoutException();

            var changedEvent = task.Result;
            Assert.AreEqual(syncCommand.AggregateId, changedEvent.AggregateId);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }




        [Then]
        public void When_command_executed_asynchroniosly_Then_aggregate_doesnt_have_events_after_finish()
        {
            var  syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }

    internal class AggregateChangedEventNotification
    {
        public Guid AggregateId { get; set; }
    }
}
