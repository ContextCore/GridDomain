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
    public class AsyncExecute_without_timeout_using_node_defaults : NodeTestKit
    {
        public AsyncExecute_without_timeout_using_node_defaults(ITestOutputHelper output) : base(output, CreateFixture()) {}

        private static NodeTestFixture CreateFixture()
        {
            return new NodeTestFixture(new SampleDomainContainerConfiguration(),
                new SampleRouteMap(),
                TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            await Node.Prepare(new LongOperationCommand(1000, Guid.NewGuid()))
                      .Expect<SampleAggregateChangedEvent>()
                      .Execute()
                      .ShouldThrow<TimeoutException>();
        }
    }
}