using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class State_Saga_Should_Not_save_snapshots_on_message_process_by_default: SoftwareProgrammingStateSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SoftwareProgrammingSagaState>[] _snapshots;

        public State_Saga_Should_Not_save_snapshots_on_message_process_by_default():base(false)
        {

        }

        [OneTimeSetUp]
        public void Given_default_policy()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(), _sagaId);

            var waiter = GridNode.NewWaiter(TimeSpan.FromSeconds(100))
                                 .Expect<SagaCreatedEvent<SoftwareProgrammingSaga.States>>()
                                 .Create();

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

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory)
                                .Load<SoftwareProgrammingSagaState>(sagaStartEvent.SagaId);
        }

        [Test]
        public void Snapshots_should_be_saved_two_times()
        {
            Assert.AreEqual(0, _snapshots.Length);
        }
    }
}