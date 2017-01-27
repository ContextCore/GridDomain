using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class CommandWaiter_results_tests : SampleDomainCommandExecutionTests
    {
        private IWaitResults _results;

        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap =
                new CustomRouteMap(
                    r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }

        [OneTimeSetUp]
        public async Task When_expect_more_than_one_messages()
        {
            var cmd = new CreateAndChangeSampleAggregateCommand(100, Guid.NewGuid());

            _results = await Node.Prepare(cmd)
                                     .Expect<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId)
                                     .And<SampleAggregateCreatedEvent>(e => e.SourceId == cmd.AggregateId)
                                     .Execute(DefaultTimeout);
        }

       [Fact]
        public void Then_recieve_something()
        {
            Assert.NotNull(_results);
        }

       [Fact]
        public void Then_recieve_non_empty_collection()
        {
            CollectionAssert.IsNotEmpty(_results.All);
        }

       [Fact]
        public void Then_recieved_collection_of_expected_messages()
        {
            Assert.IsTrue(_results.Message<SampleAggregateChangedEvent>() != null && 
                          _results.Message<SampleAggregateCreatedEvent>() != null);
        }

       [Fact]
        public void Then_recieve_only_expected_messages()
        {
            Assert.IsTrue(_results.All.Count == 2);
        }
    }
}