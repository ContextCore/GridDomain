using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_async_method_called_Then_domainEvents_are_persisted : NodeTestKit
    {
        public When_async_method_called_Then_domainEvents_are_persisted(ITestOutputHelper output) : this(new NodeTestFixture(output)) {}
        protected When_async_method_called_Then_domainEvents_are_persisted(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())) {}

        [Fact]
        public async Task When_async_method_is_called_domainEvents_are_persisted()
        {
            var cmd = new PlanTitleChangeCommand(Guid.NewGuid().ToString(), 43, TimeSpan.FromMilliseconds(50));

            await Node.Prepare(cmd).Expect<BalloonTitleChanged>().Execute();

            var aggregate = await this.LoadAggregateByActor<Balloon>(cmd.AggregateId);

            Assert.Equal(cmd.Parameter.ToString(), aggregate.Title);
        }
    }
}