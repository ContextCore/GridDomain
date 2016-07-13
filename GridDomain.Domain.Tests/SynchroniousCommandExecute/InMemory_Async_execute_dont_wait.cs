using System;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_Async_execute_dont_wait : SynchroniousCommandExecutionTests
    {
        public InMemory_Async_execute_dont_wait():base(true)
        {
            
        }
        public InMemory_Async_execute_dont_wait(bool inMemory = true):base(inMemory)
        {
            
        }
        [Then]
        public void Async_execute_dont_wait()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }
}