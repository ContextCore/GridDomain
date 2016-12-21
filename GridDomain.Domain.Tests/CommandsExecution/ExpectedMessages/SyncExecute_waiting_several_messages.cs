using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class SyncExecute_waiting_several_messages : SampleDomainCommandExecutionTests
    {
        private object[] _anObject;


        public SyncExecute_waiting_several_messages() : base(true)
        {
        }

        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap =
                new CustomRouteMap(
                    r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }

        [OneTimeSetUp]
        public void When_expect_more_than_one_messages()
        {
            var syncCommand = new CreateAndChangeSampleAggregateCommand(100, Guid.NewGuid());
            var messages = new ExpectedMessage[]
            {
                Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId),
                Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, syncCommand.AggregateId)
            };

            _anObject = GridNode.Execute(new CommandPlan(syncCommand, messages)).Result as object[]; //to array            
        }

        [Then]
        public void Then_recieve_something()
        {
            Assert.NotNull(_anObject);
        }

        [Then]
        public void Then_recieve_non_empty_collection()
        {
            Assert.IsTrue(_anObject.Any());
        }

        [Then]
        public void Then_recieve_collection_of_expected_messages()
        {
            Assert.IsTrue(_anObject.Any(m => m is IMessageMetadataEnvelop<SampleAggregateChangedEvent>));
            Assert.IsTrue(_anObject.Any(m => m is IMessageMetadataEnvelop<SampleAggregateCreatedEvent>));
        }

        [Then]
        public void Then_recieve_only_expected_messages()
        {
            Assert.IsTrue(_anObject.Length == 2);
        }
    }
}