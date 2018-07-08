using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class CommandWaiter_results_tests : NodeTestKit
    {
        protected CommandWaiter_results_tests(NodeTestFixture output):base(output.Add(new BalloonDomainConfiguration())){}
        public CommandWaiter_results_tests(ITestOutputHelper output)
            : this(new NodeTestFixture(output)) {}

        private IWaitResult _result;


        [Fact]
        public async Task When_expect_more_than_one_messages()
        {
            var cmd = new InflateCopyCommand(100, Guid.NewGuid().ToString());

            _result = await Node.Prepare(cmd)
                                .Expect<BalloonTitleChanged>(e => e.SourceId == cmd.AggregateId)
                                .And<BalloonCreated>(e => e.SourceId == cmd.AggregateId)
                                .Execute();

            await Task.Delay(1000); //waiting for logs
            //Then_recieve_something()
            Assert.NotNull(_result);
            //Then_recieve_non_empty_collection()
            Assert.NotEmpty(_result.All);
            //Then_recieved_collection_of_expected_messages()
            Assert.True(_result.Message<BalloonTitleChanged>() != null
                        && _result.Message<BalloonCreated>() != null);
            //Then_recieve_only_expected_messages()
            Assert.Equal(2,_result.All.Count);
        }
    }
}