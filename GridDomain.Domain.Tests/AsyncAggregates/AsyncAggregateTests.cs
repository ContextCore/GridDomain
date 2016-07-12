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
    public class When_aggregate_raise_async_events_
    {
        [Then]
        public void It_places_continuation_in_uncommited_async_events_collection()
        {
            var aggregate = WhenRaiseAsyncEvents();
            Assert.AreEqual(1, aggregate.AsyncUncomittedEvents.Count);
        }

        [Then]
        public void Nothing_is_applied_to_aggregate_on_async_finish()
        {
            var aggregate = WhenRaiseAsyncEvents();
            Thread.Sleep(2000);
            Assert.Null(aggregate.Value);
        }

        private static SampleAggregate WhenRaiseAsyncEvents()
        {
            var aggregate = new SampleAggregate(Guid.NewGuid(), null);
            aggregate.ChangeStateAsync(42);
            return aggregate;
        }


        [Then]
        public void Then_it_results_can_be_applied_to_aggregate()
        {
            var aggregate = WhenRaiseAsyncEvents();
            var asyncEvents = aggregate.AsyncUncomittedEvents.First();
            Thread.Sleep(1500);
            aggregate.FinishAsyncExecution(asyncEvents.InvocationId);
            Assert.AreEqual("42", aggregate.Value);
        }

    }


    [TestFixture]
    class AsyncAggregateTests : SampleDomainCommandExecutionTests
    {
        public AsyncAggregateTests() : base(true)
        {
        }

        [Test]
        public void When_async_method_is_called_other_commands_can_be_executed_before_async_results()
        {
            var asyncCommand = new AsyncMethodCommand(43, Guid.NewGuid());
            var syncCommand = new ChangeAggregateCommand(42, asyncCommand.AggregateId);
            GridNode.Execute(asyncCommand);
            GridNode.Execute(syncCommand);
            Thread.Sleep(200);
            var sampleAggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            var valueAfterSyncCommand = sampleAggregate.Value;
            Thread.Sleep(1500);//allow async command to fire & process results in actors
            var valueAfterAsyncCommand = sampleAggregate.Value;

            Assert.AreEqual(syncCommand.Parameter.ToString(), valueAfterSyncCommand);
            Assert.AreEqual(asyncCommand.Parameter.ToString(), valueAfterAsyncCommand);
        }

        [Test]
        public void When_async_method_finished_produced_events_has_sagaId_from_command()
        {
            var externalCallCommand = new AsyncMethodCommand(43, Guid.NewGuid(),Guid.NewGuid());
            var domainEvent = GridNode.Execute<AggregateChangedEvent>(externalCallCommand, Timeout,
                                                    ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                                                    externalCallCommand.AggregateId)
                                                    );

            Assert.AreEqual(externalCallCommand.SagaId, domainEvent.SagaId);
        }

        [Test]
        public void When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new AsyncMethodCommand(43, Guid.NewGuid());
            GridNode.Execute(cmd);
            Thread.Sleep(1500); //allow async command to fire & process results in actors
            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.AreEqual(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}
