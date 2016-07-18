using System;
using GridDomain.Tests.DependencyInjection.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.DependencyInjection
{
    [TestFixture]
    public class Given_configured_container_When_executing_aggregate_handler : AggregatesDI
    {
        [Test]
        public void Given_configured_container_When_executing_aggregate_handler_Then_container_is_available_in_aggregate_command_handler()
        {
            var testCommand = new TestCommand(42, Guid.NewGuid());
            ExecuteAndWaitFor<TestDomainEvent>(testCommand);
        }
    }
}