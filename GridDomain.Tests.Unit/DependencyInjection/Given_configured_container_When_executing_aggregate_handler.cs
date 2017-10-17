using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.DependencyInjection.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.DependencyInjection
{
    public class Given_configured_container_When_executing_aggregate_handler : NodeTestKit
    {
        public Given_configured_container_When_executing_aggregate_handler(ITestOutputHelper helper) 
            : base(new NodeTestFixture(helper).Add(new TestAggregateDomainConfiguration())) {}

        [Fact]
        public async Task
            Given_configured_container_When_executing_aggregate_handler_Then_container_is_available_in_aggregate_command_handler()
        {
            await Node.Prepare(new TestCommand(42, Guid.NewGuid()))
                      .Expect<TestDomainEvent>()
                      .Execute();
        }
    }
}