using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SynchroniousCommandExecute;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    class AsyncAggregateTests : ExtendedNodeCommandTest
    {
        public AsyncAggregateTests() : base(true)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
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

        [Test]
        public void When_async_method_is_called_other_commands_can_be_executed_before_async_results()
        {
            var asyncCommand = new ExternalCallCommand(43, Guid.NewGuid());
            var syncCommand = new ChangeAggregateCommand(42, asyncCommand.AggregateId);
            GridNode.Execute(asyncCommand);
            GridNode.Execute(syncCommand);
            Thread.Sleep(100);
            var valueAfterSyncCommand = LoadAggregate<SampleAggregate>(syncCommand.AggregateId).Value;
            Thread.Sleep(1500);//allow async command to fire & process results in actors
            var valueAfterAsyncCommand = LoadAggregate<SampleAggregate>(syncCommand.AggregateId).Value;

            Assert.AreEqual(syncCommand.Parameter.ToString(), valueAfterSyncCommand);
            Assert.AreEqual(asyncCommand.Parameter.ToString(), valueAfterAsyncCommand);
        }

        [Test]
        public void Async_aggregate_calls_can_be_awaited()
        {
            var externalCallCommand = new ExternalCallCommand(43, Guid.NewGuid());
            GridNode.Execute(externalCallCommand,
                             Timeout,
                             ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                             externalCallCommand.AggregateId));

            var aggregate = LoadAggregate<SampleAggregate>(externalCallCommand.AggregateId);
            Assert.AreEqual(externalCallCommand.Parameter.ToString(), aggregate.Value);
        }

        [Test]
        public void Notifications_after_async_aggregate_calls_can_be_awaited()
        {
            var externalCallCommand = new ExternalCallCommand(42, Guid.NewGuid());
            GridNode.Execute(externalCallCommand,
                Timeout,
                ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,
                    externalCallCommand.AggregateId)
                );

            var aggregate = LoadAggregate<SampleAggregate>(externalCallCommand.AggregateId);
            Assert.AreEqual(externalCallCommand.Parameter.ToString(), aggregate.Value);
        }

        [Test]
        public void When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new ExternalCallCommand(43, Guid.NewGuid());
            GridNode.Execute(cmd);
            Thread.Sleep(1500); //allow async command to fire & process results in actors
            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.AreEqual(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}
