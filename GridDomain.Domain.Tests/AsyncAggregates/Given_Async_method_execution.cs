using System;
using System.Threading;
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

      
    }
}