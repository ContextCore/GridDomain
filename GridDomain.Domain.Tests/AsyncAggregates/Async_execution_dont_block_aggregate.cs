using System;
using System.Threading;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    class Async_execution_dont_block_aggregate : InMemorySampleDomainTests
    {
       
        [Test]
        public void When_async_method_is_called_other_commands_can_be_executed_before_async_results()
        {
            var aggregateId = Guid.NewGuid();
            var asyncCommand = new AsyncMethodCommand(43, Guid.NewGuid(),Guid.NewGuid(),TimeSpan.FromSeconds(3));
            var syncCommand = new ChangeSampleAggregateCommand(42, aggregateId);

           var asyncCommandTask = GridNode.Execute<SampleAggregateChangedEvent>(asyncCommand,
                                                    ExpectedMessage.Once<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),
                                                    asyncCommand.AggregateId));

            GridNode.Execute(syncCommand, Timeout,
                             ExpectedMessage.Once<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),
                             syncCommand.AggregateId)
                             );

            var sampleAggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), sampleAggregate.Value);

            var asyncResult = asyncCommandTask.Result;
            Assert.AreEqual(asyncCommand.Parameter.ToString(), asyncResult.Value);
        }
    }
}