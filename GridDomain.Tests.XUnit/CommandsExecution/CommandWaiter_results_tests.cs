using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class CommandWaiter_results_tests : NodeTestKit
    {
        public CommandWaiter_results_tests(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new SampleDomainContainerConfiguration(), CreateMap())) {}

        private IWaitResults _results;

        private static IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap = new CustomRouteMap(r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }

        [Fact]
        public async Task When_expect_more_than_one_messages()
        {
            var cmd = new CreateAndChangeSampleAggregateCommand(100, Guid.NewGuid());

            _results = await Node.Prepare(cmd)
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                 .And<SampleAggregateCreatedEvent>(e => e.SourceId == cmd.AggregateId)
                                 .Execute();
            //Then_recieve_something()
            Assert.NotNull(_results);
            //Then_recieve_non_empty_collection()
            Assert.NotEmpty(_results.All);
            //Then_recieved_collection_of_expected_messages()
            Assert.True(_results.Message<SampleAggregateChangedEvent>() != null
                        && _results.Message<SampleAggregateCreatedEvent>() != null);
            //Then_recieve_only_expected_messages()
            Assert.True(_results.All.Count == 2);
        }
    }
}