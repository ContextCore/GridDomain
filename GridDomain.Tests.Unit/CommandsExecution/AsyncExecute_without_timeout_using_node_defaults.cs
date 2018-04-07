using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class AsyncExecute_without_timeout_using_node_defaults : NodeTestKit
    {
        protected AsyncExecute_without_timeout_using_node_defaults(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())){}

        public AsyncExecute_without_timeout_using_node_defaults(ITestOutputHelper output) : this(
            new NodeTestFixture(output,
                                null,
                                TimeSpan.FromMilliseconds(100))) {}

        [Fact]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            await Node.Prepare(new PlanTitleWriteCommand(1000, Guid.NewGuid().ToString()))
                      .Expect<BalloonTitleChanged>()
                      .Execute()
                      .ShouldThrow<TimeoutException>();
        }
    }
}