using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.DependencyInjection.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.DependencyInjection
{
    [TestFixture]
    public class Given_configured_container_When_executing_aggregate_handler : AggregatesDI
    {
        [Test]
        public async Task Given_configured_container_When_executing_aggregate_handler_Then_container_is_available_in_aggregate_command_handler()
        {
            var testCommand = new TestCommand(42, Guid.NewGuid());

            await GridNode.Prepare(testCommand)
                          .Expect<TestDomainEvent>()
                          .Execute();
        }
    }
}