using System;
using System.Threading;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class Given_async_events_execute : InMemorySampleDomainTests
    {

        [Test]
        public void When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new AsyncMethodCommand(43, Guid.NewGuid());
            GridNode.Execute(cmd);
            Thread.Sleep(5000); //allow async command to fire & process results in actors
            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.AreEqual(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}