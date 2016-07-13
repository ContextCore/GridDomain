using System;
using System.Threading;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    class Given_Async_method_execution : InMemorySampleDomainTests
    {
       
        [Test]
        public void When_async_method_is_called_other_commands_can_be_executed_before_async_results()
        {
            var aggregateId = Guid.NewGuid();
            var asyncCommand = new AsyncMethodCommand(43, Guid.NewGuid(),Guid.NewGuid(),TimeSpan.FromSeconds(3));
            var syncCommand = new ChangeAggregateCommand(42, aggregateId);

           var asyncCommandTask = GridNode.Execute<AggregateChangedEvent>(asyncCommand,
                                                    ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                                                    asyncCommand.AggregateId));

            GridNode.Execute<AggregateChangedEvent>(syncCommand, Timeout,
                                                    ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                                                    syncCommand.AggregateId)
                                                    );

            var sampleAggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), sampleAggregate.Value);

            var asyncResult = asyncCommandTask.Result;
            Assert.AreEqual(asyncCommand.Parameter.ToString(), asyncResult.Value);
        }

      
    }
}