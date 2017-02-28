using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.DependencyInjection.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.DependencyInjection
{
    public class Given_configured_container_When_executing_aggregate_handler : AggregatesDI
    {
        public Given_configured_container_When_executing_aggregate_handler(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task
            Given_configured_container_When_executing_aggregate_handler_Then_container_is_available_in_aggregate_command_handler
            ()
        {
            var testCommand = new TestCommand(42, Guid.NewGuid());

            await Node.Prepare(testCommand)
                      .Expect<TestDomainEvent>()
                      .Execute();
        }
    }
}