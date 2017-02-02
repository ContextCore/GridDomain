using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class When_async_method_called_Then_domainEvents_are_persisted : SampleDomainCommandExecutionTests
    {
        public When_async_method_called_Then_domainEvents_are_persisted(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new AsyncMethodCommand(43, Guid.NewGuid(), Guid.Empty, TimeSpan.FromMilliseconds(50));

            await Node.Prepare(cmd)
                      .Expect<SampleAggregateChangedEvent>()
                      .Execute();

            var aggregate = await this.LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.Equal(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}