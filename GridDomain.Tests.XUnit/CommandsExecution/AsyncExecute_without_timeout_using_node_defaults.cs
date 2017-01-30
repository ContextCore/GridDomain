using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class AsyncExecute_without_timeout_using_node_defaults : NodeTestKit
    {
       [Fact]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            await Node.Prepare(syncCommand)
                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                .Execute()
                .ShouldThrow<TimeoutException>();
        }

        public AsyncExecute_without_timeout_using_node_defaults(ITestOutputHelper output) : base(output, CreateFixture())
        {
        }

        private static NodeTestFixture CreateFixture()
        {
            return new NodeTestFixture(new SampleDomainContainerConfiguration(),
                                       new SampleRouteMap(),
                                       TimeSpan.FromMilliseconds(100));
        }
    }
}