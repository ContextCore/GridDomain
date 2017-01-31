using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    //different fixtures from static method ? 
    public class When_executing_commands_and_aggregate_raises_an_exception: NodeTestKit, IClassFixture<SampleDomainFixture>
    {

        public When_executing_commands_and_aggregate_raises_an_exception(ITestOutputHelper helper, SampleDomainFixture fixture):base(helper, fixture)
        {
        }

      

       [Fact]
        public async Task When_does_not_expect_fault_and_it_accures_wait_times_out()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                      .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                      .Execute(TimeSpan.FromMilliseconds(50))
                      .ShouldThrow<TimeoutException>();
        }


       [Fact]
        public async Task When_expected_optional_fault_does_not_occur_wait_is_successfull()
        {
            var syncCommand = new LongOperationCommand(101, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                                    .Or<IFault>(f => (f.Message as DomainEvent)?.SourceId == syncCommand.AggregateId)
                                    .Execute(TimeSpan.FromSeconds(1000));

            var evt = res.Message<AggregateChangedEventNotification>();
            Assert.Equal(syncCommand.AggregateId, evt.AggregateId);
        }

       [Fact]
        public async Task When_fault_is_produced_when_publish_command_with_base_type()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(101, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                          .Expect<AggregateChangedEventNotification>(e => e.AggregateId == syncCommand.AggregateId)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }

       [Fact]
        public async Task When_aggregate_throws_fault_it_is_handled_without_implicit_registration()
        {
            //will throw exception in aggregate and in message handler
            var syncCommand = new AsyncFaultWithOneEventCommand(50, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<AggregateChangedEventNotification>()
                                .Or<IFault<SampleAggregateChangedEvent>>()
                                .Execute(false);

            Assert.NotNull(res.Message<IFault<AsyncFaultWithOneEventCommand>>());
        }


       [Fact]
        public async Task When_fault_was_received_and_failOnFaults_is_set_results_raised_an_error()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(100, Guid.NewGuid());
            await Node.Prepare(syncCommand)
                          .Expect<AggregateChangedEventNotification>()
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }
    }
}