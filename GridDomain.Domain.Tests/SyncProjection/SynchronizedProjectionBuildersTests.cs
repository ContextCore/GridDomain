using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using GridDomain.Tests.SyncProjection.SampleDomain;
using ChangeAggregateCommand = GridDomain.Tests.SynchroniousCommandExecute.ChangeAggregateCommand;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.SyncProjection
{
    [TestFixture]
    public class SynchronizedProjectionBuildersTests : NodeCommandsTest
    {
        private ExpectedMessagesRecieved _processedEvents;
        private CQRS.ICommand[] _allCommands;

        public SynchronizedProjectionBuildersTests():
            base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "ProjectionBuilders", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromMinutes(1);
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container  = new UnityContainer();
            var system = ActorSystemFactory.CreateActorSystem(akkaConf);
            CompositionRoot.Init(container, system, dbConfig, TransportMode.Standalone);
            container.RegisterAggregate<SampleAggregate, TestAggregatesCommandHandler>();

            return new GridDomainNode(container, 
                                      new TestRouteMap(new UnityServiceLocator(container)), 
                                      TransportMode.Standalone, system);
        }

        [TestFixtureSetUp]
        public void When_execute_many_commands_for_create_and_update()
        {
            var createCommands = Enumerable.Range(0, 10).Select(r => new CreateAggregateCommand(101, Guid.NewGuid())).ToArray();
            var aggregateIds = createCommands.Select(c => c.AggregateId).ToArray();
            var updateCommands = Enumerable.Range(0, 20).Select(r => new ChangeAggregateCommand(aggregateIds.RandomElement(), 102)).ToArray();

            _allCommands = createCommands.Cast<CQRS.ICommand>().Concat(updateCommands).ToArray();

            _processedEvents = ExecuteAndWaitForMany<AggregateCreatedEvent, AggregateChangedEvent>(createCommands.Length,
                                                                                                   updateCommands.Length,
                                                                                                   _allCommands);
        }

        [Test]
        public void All_events_related_to_one_aggregate_processed_time_should_be_only_increasing()
        {
            AllEventsForOneAggregate_should_be_ordered_by(e => e.History.ElapsedTicksFromAppStart);
        }

        [Test]
        public void All_events_related_to_one_aggregate_processed_number_should_be_only_increasing()
        {
            AllEventsForOneAggregate_should_be_ordered_by(e => e.History.SequenceNumber);
        }

        [Test]
        public void All_events_related_to_one_aggregate_processor_should_be_the_same()
        {
            AllEventsForOneAggregate_should_be_ordered_by(e => e.History.ProjectionGroupHashCode);
        }

        private void AllEventsForOneAggregate_should_be_ordered_by(Func<IHaveProcessingHistory, long> valueSelector)
        {
            foreach (var eventsForOneAggregate in _processedEvents.Recieved.Cast<DomainEvent>().GroupBy(e => e.SourceId))
            {
                CheckOrderedValues(eventsForOneAggregate.Cast<IHaveProcessingHistory>(),
                                   valueSelector);
            }
        }

        private void CheckOrderedValues<TElem>(IEnumerable<TElem> elements, Func<TElem, long> valueSelector)
        {
            long prevEventTime = 0;
            foreach (var element in elements)
            {
                var currentValue = valueSelector(element);
                Assert.GreaterOrEqual(currentValue, prevEventTime);
                prevEventTime = currentValue;
            }
        }

    }
}
