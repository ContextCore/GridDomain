using System;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class Async_execute_dont_wait : SampleDomainCommandExecutionTests
    {
        public Async_execute_dont_wait():base(true)
        {
            
        }
        public Async_execute_dont_wait(bool inMemory = true):base(inMemory)
        {
            
        }
        [Then]
        public void Async_execute_dont_wait_for_command_finish()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }
}