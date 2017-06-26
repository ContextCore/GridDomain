using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_async_method_called_Then_domainEvents_are_persisted : SampleDomainCommandExecutionTests
    {
        public When_async_method_called_Then_domainEvents_are_persisted(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new PlanTitleChangeCommand(43, Guid.NewGuid(), Guid.Empty, TimeSpan.FromMilliseconds(50));

            await Node.Prepare(cmd).Expect<BalloonTitleChanged>().Execute();

            var aggregate = await this.LoadAggregate<Balloon>(cmd.AggregateId);

            Assert.Equal(cmd.Parameter.ToString(), aggregate.Title);
        }
    }
}