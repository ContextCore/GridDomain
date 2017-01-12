using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class When_async_method_called_Then_domainEvents_are_persisted : InMemorySampleDomainTests
    {

        [Test]
        public async Task When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new AsyncMethodCommand(43, Guid.NewGuid(),Guid.Empty,TimeSpan.FromMilliseconds(50));

            await GridNode.PrepareCommand(cmd)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute();

            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.AreEqual(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}