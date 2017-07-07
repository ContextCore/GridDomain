using System;
using System.Collections.Generic;
using System.Reflection;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Serialization;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using Ploeh.AutoFixture;
using Xunit;

namespace GridDomain.Tests.Unit
{
    public class Types_should_be_deserializable : TypesDeserializationTest
    {
        private readonly ExtendedActorSystem _system;
        protected override ObjectDeserializationChecker Checker { get; } 

        public Types_should_be_deserializable()
        {
            Fixture.Register<ICommand>(() => new FakeCommand(Guid.NewGuid()));
            Fixture.Register<Command>(() => new FakeCommand(Guid.NewGuid()));
            Fixture.Register<DomainEvent>(() => new BalloonCreated("1", Guid.NewGuid()));
            Fixture.Register<Exception>(() => new Exception("test exception"));

            _system = (ExtendedActorSystem)ActorSystem.Create("test");
            _system.InitDomainEventsSerialization(new EventsAdaptersCatalog());

            Checker = GetChecker(new DomainEventsJsonAkkaSerializer(_system), new WireSerializer(_system));
        }

        private ObjectDeserializationChecker GetChecker(params Serializer[] ser)
        {
            return new ObjectDeserializationChecker(null, ser);
        }
        protected override IEnumerable<Type> ExcludeTypes => new List<Type>();

        private class FakeCommand : Command
        {
            public FakeCommand(Guid aggregateId) : base(aggregateId) {}
        }

        protected override Assembly[] AllAssemblies { get; } = {
                                                                   Assembly.GetAssembly(typeof(GridDomainNode)),
                                                                   Assembly.GetAssembly(typeof(QuartzSchedulerConfiguration)),
                                                                   Assembly.GetAssembly(typeof(SagaReceivedMessage<>)),
                                                                   Assembly.GetAssembly(typeof(Balloon)),
                                                                   Assembly.GetAssembly(typeof(ISagaCreatorCatalog<>)),
                                                                   Assembly.GetAssembly(typeof(DomainEvent)),
                                                                   Assembly.GetAssembly(typeof(ExecutionOptions))
                                                               };

        [Fact]
        public void Aggregates_from_all_assemblies_should_be_deserializable_by_json()
        {
            var checker = GetChecker(new DomainEventsJsonAkkaSerializer(_system));
            CheckAllChildrenOf<IAggregate>(checker, AllAssemblies);
        }

        [Fact]
        public void Commands_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {
            CheckAllChildrenOf<ICommand>(Checker, AllAssemblies);
        }

        [Fact]
        public void DomainEvents_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {
            CheckAllChildrenOf<DomainEvent>(Checker, AllAssemblies);
        }

        [Fact]
        public void SagaState_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {
            CheckAllChildrenOf<ISagaState>(Checker, AllAssemblies);
        }


        [Fact]
        public void Saga_commands_aggregates_and_events_should_be_deserializable_by_json()
        {
            var state = Fixture.Create<SoftwareProgrammingState>();
            var aggregate = new SagaStateAggregate<SoftwareProgrammingState>(state);
            aggregate.PersistAll();
            var checker = GetChecker(new DomainEventsJsonAkkaSerializer(_system));
            checker.AfterRestore = o =>
                                   {
                                       ((IMemento) o).Version = 0;
                                       ((Aggregate) o).PersistAll();
                                   };

            CheckResults(checker.IsRestorable(aggregate, out string difference1) ?
                             RestoreResult.Ok(aggregate.GetType()) :
                             RestoreResult.Diff(aggregate.GetType(), difference1));
        }

        [Fact]
        public void MessageMetadata_classes_should_be_deserializable()
        {
            CheckAll<object>(Checker, typeof(MessageMetadata));
        }


        [Fact]
        public void Scheduler_job_types_from_all_assemblies_should_be_deserializable()
        {
            CheckAll<object>(Checker, typeof(ExecutionOptions), typeof(ScheduleKey));
        }
    }
}