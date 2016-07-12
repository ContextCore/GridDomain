using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class When_WaitExecute_of_async_aggregate_method_with_fault_wait_by_Node : SampleDomainCommandExecutionTests
    {

        public When_WaitExecute_of_async_aggregate_method_with_fault_wait_by_Node() : base(true)
        {

        }
       
        [Then]
        public void Then_execute_throws_exception_from_aggregate()
        {
            var syncCommand = new AlwaysFaultAsyncCommand(Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEvent>(e => e.SourceId,syncCommand.AggregateId);

            Assert.Throws<SampleAggregateException>(() => 
                        GridNode.Execute<AggregateChangedEvent>(syncCommand,Timeout,expectedMessage));
        }

     
    }
}