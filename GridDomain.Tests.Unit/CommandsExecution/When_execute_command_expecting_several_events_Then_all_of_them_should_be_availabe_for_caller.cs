using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller : SampleDomainCommandExecutionTests
    {
        private object[] _allReceivedMessages;

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
            var syncCommand = new CreateAndChangeSampleAggregateCommand(100, Guid.NewGuid());
            var waitResults = await GridNode.PrepareCommand(syncCommand)
                                            .Expect<SampleAggregateChangedEvent>()
                                            .And<SampleAggregateCreatedEvent>()
                                            .Execute();

            _allReceivedMessages = waitResults.All.ToArray();
        }

        [Then]
        public void Then_recieve_something()
        {
            Assert.NotNull(_allReceivedMessages);
        }

        [Then]
        public void Then_recieve_non_empty_collection()
        {
            Assert.IsTrue(_allReceivedMessages.Any());
        }

        [Then]
        public void Then_recieve_collection_of_expected_messages()
        {
            Assert.IsTrue(_allReceivedMessages.Any(m => m is IMessageMetadataEnvelop<SampleAggregateChangedEvent>));
            Assert.IsTrue(_allReceivedMessages.Any(m => m is IMessageMetadataEnvelop<SampleAggregateCreatedEvent>));
        }

        [Then]
        public void Then_recieve_only_expected_messages()
        {
            Assert.IsTrue(_allReceivedMessages.Length == 2);
        }
    }
}