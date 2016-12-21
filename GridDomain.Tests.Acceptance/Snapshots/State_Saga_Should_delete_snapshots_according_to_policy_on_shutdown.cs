using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class State_Saga_Should_delete_snapshots_according_to_policy_on_shutdown: SoftwareProgrammingStateSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SoftwareProgrammingSagaState>[] _snapshots;

        public State_Saga_Should_delete_snapshots_according_to_policy_on_shutdown():base(false)
        {
            
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                base.CreateConfiguration(),
                SagaConfiguration.State<SoftwareProgrammingSaga,
                                             SoftwareProgrammingSagaState,
                                             SoftwareProgrammingSagaFactory,
                                             GotTiredEvent>
                                             (SoftwareProgrammingSaga.Descriptor,
                                              () => new SnapshotsPersistencePolicy(TimeSpan.FromSeconds(10), 2, 3),
                                              SoftwareProgrammingSagaState.FromSnapshot));
        }


        [OneTimeSetUp]
        public void Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(), _sagaId);

            var w = GridNode.NewWaiter()
                                 .Expect<SagaCreatedEvent<SoftwareProgrammingSaga.States>>()
                                 .Create();

            Publisher.Publish(sagaStartEvent);
            w.Wait();

            var sagaActorRef = LookupStateSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>(_sagaId);
            Watch(sagaActorRef);
            sagaActorRef.Tell(new NotifyOnPersistenceEvents(TestActor), TestActor);



            var sagaContinueEventA = new CoffeMakeFailedEvent(_sagaId,
                                                             sagaStartEvent.PersonId,
                                                             BusinessDateTime.UtcNow,
                                                            _sagaId);

            var sagaContinueEventB = new SleptWellEvent(_sagaId,
                                                        sagaStartEvent.LovelySofaId,
                                                        _sagaId);

            var waiter = GridNode.NewWaiter()
                                 .Expect<SagaTransitionEvent<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>>(e => e.State == SoftwareProgrammingSaga.States.Coding)
                                 .And<SagaTransitionEvent<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>>(e => e.State == SoftwareProgrammingSaga.States.Sleeping)
                                 .Create();

            Publisher.Publish(sagaContinueEventA);
            Publisher.Publish(sagaContinueEventB);

            waiter.Wait();

            sagaActorRef.Tell(GracefullShutdownRequest.Instance, TestActor);

            FishForMessage<Terminated>(m => true);

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, 
                                                         GridNode.AggregateFromSnapshotsFactory)
                                                         .Load<SoftwareProgrammingSagaState>(_sagaId);
        }

        [Test]
        public void Only_2_Snapshots_should_left()
        {
            Assert.AreEqual(2, _snapshots.Length);
        }

        [Test]
        public void Restored_aggregates_should_have_same_ids()
        {
           Assert.True(_snapshots.All(s => s.Aggregate.Id == _sagaId));
        }

        [Test]
        public void Last_Snapshots_should_have_coding_state_from_last_event()
        {
            Assert.AreEqual(SoftwareProgrammingSaga.States.Coding, _snapshots.Last().Aggregate.MachineState);
        }

        [Test]
        public void First_Snapshots_should_have_coding_state_from_last_event()
        {
            Assert.AreEqual(SoftwareProgrammingSaga.States.Sleeping, _snapshots.First().Aggregate.MachineState);
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}