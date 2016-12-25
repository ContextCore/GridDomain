using System;
using System.Threading.Tasks;
using GridDomain.Tests.DependencyInjection.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.DependencyInjection
{
    [TestFixture]
    public class Given_configured_container_When_executing_aggregate_handler : AggregatesDI
    {
        [Test]
        public async Task Given_configured_container_When_executing_aggregate_handler_Then_container_is_available_in_aggregate_command_handler()
        {
            var testCommand = new TestCommand(42, Guid.NewGuid());

            await GridNode.PrepareCommand(testCommand)
                          .Expect<TestDomainEvent>()
                          .Execute();
        }
    }
}