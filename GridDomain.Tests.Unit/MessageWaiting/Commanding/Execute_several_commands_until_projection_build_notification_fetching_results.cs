using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting.Commanding
{
    [TestFixture]
    public class Execute_several_commands_until_projection_build_notification_fetching_results : SampleDomainCommandExecutionTests
    {
        private AggregateChangedEventNotification _changedEventA;
        private AggregateChangedEventNotification _changedEventB;
        private AggregateChangedEventNotification _changedEventC;
        private SampleAggregate _aggregateA;
        private LongOperationCommand _syncCommandA;
        private LongOperationCommand _syncCommandB;
        private LongOperationCommand _syncCommandC;
        private IWaitResults _results;


        [OneTimeSetUp]
        public void Given_command_executes_with_waiter_When_fetching_results()
        {
            _syncCommandA = new LongOperationCommand(100, Guid.NewGuid());
            _syncCommandB = new LongOperationCommand(200, Guid.NewGuid());
            _syncCommandC = new LongOperationCommand(150, Guid.NewGuid());


            _results = GridNode.NewCommandWaiter(Timeout)
                               .Expect<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommandA.AggregateId)
                               .And<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommandB.AggregateId)
                               .And<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommandC.AggregateId)
                               .Create()
                               .Execute(_syncCommandA, _syncCommandB, _syncCommandC)
                               .Result;

            _changedEventA = _results.Message<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommandA.AggregateId);
            _changedEventB = _results.Message<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommandB.AggregateId);
            _changedEventC = _results.Message<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommandC.AggregateId);

            _aggregateA = LoadAggregate<SampleAggregate>(_syncCommandA.AggregateId);
        }

        [Test]
        public void Results_contains_received_messages()
        {
            CollectionAssert.IsNotEmpty(_results.All);
        }

        [Test]
        public void Results_contains_all_requested_messages()
        {
            CollectionAssert.AreEquivalent(new object[] { _changedEventA , _changedEventB, _changedEventC},
                                          _results.All.Cast<IMessageMetadataEnvelop>().Select(e => e.Message));
        }

        [Test]
        public void Emmited_eventA_has_correct_id()
        {
            Assert.AreEqual(_syncCommandA.AggregateId, _changedEventA?.AggregateId);
        }

        [Test]
        public void Emmited_eventB_has_correct_id()
        {
            Assert.AreEqual(_syncCommandB.AggregateId, _changedEventB?.AggregateId);
        }

        [Test]
        public void Emmited_eventC_has_correct_id()
        {
            Assert.AreEqual(_syncCommandC.AggregateId, _changedEventC?.AggregateId);
        }

        [Test]
        public void Aggregate_has_correct_state_from_command()
        {
            Assert.AreEqual(_syncCommandA.Parameter.ToString(), _aggregateA.Value);
        }

    }
}