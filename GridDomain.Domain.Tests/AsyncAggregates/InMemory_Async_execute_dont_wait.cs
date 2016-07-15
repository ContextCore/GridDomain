using System;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class InMemory_Async_execute_dont_wait : SampleDomainCommandExecutionTests
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
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }
}