using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller :
        SampleDomainCommandExecutionTests
    {
        public When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller(
            ITestOutputHelper output) : base(output) {}

        private object[] _allReceivedMessages;

        [Fact]
        public async Task When_expect_more_than_one_messages()
        {
            var syncCommand = new CreateAndChangeSampleAggregateCommand(100, Guid.NewGuid());
            var waitResults =
                await
                    Node.Prepare(syncCommand)
                        .Expect<SampleAggregateChangedEvent>()
                        .And<SampleAggregateCreatedEvent>()
                        .Execute();

            _allReceivedMessages = waitResults.All.ToArray();
            //Then_recieve_something()
            Assert.NotNull(_allReceivedMessages);
            //Then_recieve_non_empty_collection()
            Assert.True(_allReceivedMessages.Any());
            //Then_recieve_collection_of_expected_messages()
            Assert.True(_allReceivedMessages.Any(m => m is IMessageMetadataEnvelop<SampleAggregateChangedEvent>));
            Assert.True(_allReceivedMessages.Any(m => m is IMessageMetadataEnvelop<SampleAggregateCreatedEvent>));
            //Then_recieve_only_expected_messages()
            Assert.True(_allReceivedMessages.Length == 2);
        }
    }
}