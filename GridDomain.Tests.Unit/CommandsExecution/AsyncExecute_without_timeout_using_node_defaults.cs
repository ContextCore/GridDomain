using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class AsyncExecute_without_timeout_using_node_defaults : NodeTestKit
    {
        public AsyncExecute_without_timeout_using_node_defaults(ITestOutputHelper output) : base(output,
            new NodeTestFixture(new BalloonDomainConfiguration(),
                                TimeSpan.FromMilliseconds(100))) {}

        [Fact]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            await Node.Prepare(new PlanTitleWriteCommand(1000, Guid.NewGuid()))
                      .Expect<BalloonTitleChanged>()
                      .Execute()
                      .ShouldThrow<TimeoutException>();
        }
    }
}