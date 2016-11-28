using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Instance_Saga_Should_delete_snapshots_according_to_policy_on_shutdown: SoftwareProgrammingInstanceSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SagaDataAggregate<SoftwareProgrammingSagaData>>[] _snapshots;
        private readonly SnapshotsSaveAfterEachMessagePolicy _snapshotsSavePolicy = 
                                   new SnapshotsSaveAfterEachMessagePolicy(4);
            

        public Instance_Saga_Should_delete_snapshots_according_to_policy_on_shutdown():base(false)
        {
            
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => base.CreateConfiguration().Register(c),
                c => c.Register(SagaConfiguration.Instance<SoftwareProgrammingSaga,
                                SoftwareProgrammingSagaData,
                                SoftwareProgrammingSagaFactory,
                                GotTiredEvent,
                                SleptWellEvent>(SoftwareProgrammingSaga.Descriptor,
                                () => _snapshotsSavePolicy)));
        }

        [OneTimeSetUp]
        public void Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(), _sagaId);

            var wait = GridNode.NewWaiter()
                               .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                               .Create();

            Publisher.Publish(sagaStartEvent);

            wait.Wait();

            var sagaActorRef = LookupInstanceSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(_sagaId);

            sagaActorRef.Tell(new NotifyOnPersistenceEvents(TestActor), TestActor);

            var sagaContinueEventA = new CoffeMakeFailedEvent(_sagaId,
                                                             sagaStartEvent.PersonId,
                                                             BusinessDateTime.UtcNow,
                                                            _sagaId);

            var sagaContinueEventB = new SleptWellEvent(_sagaId,
                                                        sagaStartEvent.LovelySofaId,
                                                        _sagaId);

            var waiter = GridNode.NewWaiter()
                                 .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(e => (e.Message as CoffeMakeFailedEvent)?.SourceId == _sagaId)
                                 .And<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(e => (e.Message as SleptWellEvent)?.SourceId == _sagaId)
                                 .Create();

            Publisher.Publish(sagaContinueEventA);
            Publisher.Publish(sagaContinueEventB);

            waiter.Wait();

            Watch(sagaActorRef);
            sagaActorRef.Tell(GracefullShutdownRequest.Instance, TestActor);

            FishForMessage<Terminated>(m => true,TimeSpan.FromDays(1));
            Thread.Sleep(1000);
            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, 
                                                         GridNode.AggregateFromSnapshotsFactory)
                                                         .Load<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);

            Console.WriteLine(_snapshotsSavePolicy.ToPropsString());
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
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Coding),_snapshots.Last().Aggregate.Data.CurrentStateName);
        }

        [Test]
        public void First_Snapshots_should_have_coding_state_from_first_event()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Sleeping), _snapshots.First().Aggregate.Data.CurrentStateName);
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}