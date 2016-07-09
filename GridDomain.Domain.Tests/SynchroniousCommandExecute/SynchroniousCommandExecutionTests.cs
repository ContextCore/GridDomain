using System;
using System.Diagnostics;
using System.Threading;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.SyncProjection.SampleDomain;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    class SynchroniousCommandExecutionTests : ExtendedNodeCommandTest
    {
        protected override TimeSpan Timeout => Debugger.IsAttached
            ? TimeSpan.FromMinutes(10)
            : TimeSpan.FromSeconds(5);

        protected override IContainerConfiguration CreateConfiguration()
        {
            return  new CustomContainerConfiguration(
                               c => c.RegisterAggregate<SampleAggregate, TestAggregatesCommandHandler>(),
                               c => c.RegisterInstance(new InMemoryQuartzConfig()),
                               c => c.RegisterType<AggregateCreatedProjectionBuilder>(),
                               c => c.RegisterType<SampleProjectionBuilder>());
        }

        protected override IMessageRouteMap CreateMap()
        {
            var container = new UnityContainer();
            container.Register(CreateConfiguration());
            return new TestRouteMap(new UnityServiceLocator(container));
        }

        [Then]
        public void SyncExecute_until_aggregate_event_wait_by_Node()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand,
                            Timeout,
                            ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                                                                        syncCommand.AggregateId));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Then]
        public void SyncExecute_until_projection_build_event_wait_by_Node()
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
        public void SyncExecute_until_aggregate_event_wait_by_caller()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            var task = GridNode.Execute<AggregateChangedEvent>(syncCommand,
                                                               ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                                                                                                           syncCommand.AggregateId)
                                                               );
            if (!task.Wait(Timeout))
                throw new TimeoutException();
       
            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        [Then]
        public void SyncExecute_until_projection_build_event_wait_by_caller()
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
        public void Async_execute_dont_wait()
        {
            var  syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }

        public SynchroniousCommandExecutionTests() : base(true)
        {
        }


        public SynchroniousCommandExecutionTests(bool inMemory) : base(inMemory)
        {
        }
    }

    [TestFixture]
    class SynchroniousCommandInMemoryExecutionTests : SynchroniousCommandExecutionTests
    {
        public SynchroniousCommandInMemoryExecutionTests() : base(true)
        {
        }
    }

    [TestFixture]
    class SynchroniousCommandPersistentExecutionTests : SynchroniousCommandExecutionTests
    {
        public SynchroniousCommandPersistentExecutionTests() : base(false)
        {
        }
    }

    internal class AggregateChangedEventNotification
    {
        public Guid AggregateId { get; set; }
    }
}
