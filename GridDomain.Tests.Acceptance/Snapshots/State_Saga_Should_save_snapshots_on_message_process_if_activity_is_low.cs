using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tools.Repositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class State_Saga_Should_save_snapshots_on_message_process_if_activity_is_low: SoftwareProgrammingStateSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SoftwareProgrammingSagaState>[] _snapshots;

        public State_Saga_Should_save_snapshots_on_message_process_if_activity_is_low():base(false)
        {

        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => base.CreateConfiguration().Register(c),
                c => SagaConfiguration.State<SoftwareProgrammingSaga,
                                             SoftwareProgrammingSagaState,
                                             SoftwareProgrammingSagaFactory,
                                             GotTiredEvent>
                                             (SoftwareProgrammingSaga.Descriptor, () => new TestSnapshotsSavePolicy()));
        }

        [OneTimeSetUp]
        public void Given_default_policy()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(), _sagaId);

            var waiter = GridNode.NewWaiter()
                                 .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                                 .Create(TimeSpan.FromSeconds(100));

            Publisher.Publish(sagaStartEvent);
            waiter.Wait();

            var sagaContinueEvent = new CoffeMakeFailedEvent(_sagaId,
                                                             sagaStartEvent.PersonId,
                                                             BusinessDateTime.UtcNow,
                                                            _sagaId);

            var waiterB = GridNode.NewWaiter(Timeout)
                                  .Expect<SagaTransitionEvent<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>>()
                                  .Create();

            Publisher.Publish(sagaContinueEvent);

            waiterB.Wait();


            //saving snapshot
            Thread.Sleep(200);

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString)
                                .Load<SoftwareProgrammingSagaState>(sagaStartEvent.SagaId);
        }

        [Test]
        public void Snapshots_should_be_saved_two_times()
        {
            Assert.AreEqual(2, _snapshots.Length);
        }

        [Test]
        public void Restored_saga_state_should_have_correct_ids()
        {
            Assert.True(_snapshots.All(s => s.Aggregate.Id == _sagaId));
        }

        [Test]
        public void First_snapshot_should_have_state_from_first_event()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.States.MakingCoffee), _snapshots.First().Aggregate.MachineState);
        }

        [Test]
        public void Second_snapshot_should_have_parameters_from_second_command()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.States.Sleeping), _snapshots.Skip(1).First().Aggregate.MachineState);
        }
    }
}