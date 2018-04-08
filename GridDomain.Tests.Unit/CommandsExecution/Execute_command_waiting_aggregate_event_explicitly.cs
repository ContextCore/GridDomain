using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution {
    public class Execute_command_waiting_aggregate_event_explicitly : NodeTestKit
    {
        public Execute_command_waiting_aggregate_event_explicitly(ITestOutputHelper output) : base(new NodeTestFixture(output).Add(new BalloonDomainConfiguration())) {}
        
        [Fact]
        public async Task MessageWaiter_after_cmd_execute_should_waits_until_aggregate_event()
        {
            var cmd = new PlanTitleWriteCommand(100, Guid.NewGuid());
            var waiter = Node.NewExplicitWaiter(TimeSpan.FromSeconds(100))
                             .Expect<MessageMetadataEnvelop>(e => (e.Message as BalloonTitleChanged)?.SourceId == cmd.AggregateId)
                             .Create();

            await Node.Execute(cmd);

            var res = await waiter;

            Assert.Equal(cmd.Parameter.ToString(), res.Message<BalloonTitleChanged>().Value);
        }
    }
}