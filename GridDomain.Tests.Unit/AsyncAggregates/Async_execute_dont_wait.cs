using System;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.AsyncAggregates
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