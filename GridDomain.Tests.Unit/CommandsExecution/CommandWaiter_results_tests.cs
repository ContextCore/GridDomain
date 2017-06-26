using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class CommandWaiter_results_tests : NodeTestKit
    {
        public CommandWaiter_results_tests(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new BalloonDomainConfiguration(), CreateMap())) {}

        private IWaitResult _result;

        private static IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap = new CustomRouteMap(r => r.RegisterAggregate(BalloonCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }

        [Fact]
        public async Task When_expect_more_than_one_messages()
        {
            var cmd = new InflateCopyCommand(100, Guid.NewGuid());

            _result =
                await
                    Node.Prepare(cmd)
                        .Expect<BalloonTitleChanged>(e => e.SourceId == cmd.AggregateId)
                        .And<BalloonCreated>(e => e.SourceId == cmd.AggregateId)
                        .Execute();
            //Then_recieve_something()
            Assert.NotNull(_result);
            //Then_recieve_non_empty_collection()
            Assert.NotEmpty(_result.All);
            //Then_recieved_collection_of_expected_messages()
            Assert.True(_result.Message<BalloonTitleChanged>() != null
                        && _result.Message<BalloonCreated>() != null);
            //Then_recieve_only_expected_messages()
            Assert.True(_result.All.Count == 2);
        }
    }
}