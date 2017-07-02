using System;
using System.Collections.Generic;
using System.Reflection;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
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
        public Types_should_be_deserializable()
        {
            Fixture.Register<ICommand>(() => new FakeCommand(Guid.NewGuid()));
            Fixture.Register<Command>(() => new FakeCommand(Guid.NewGuid()));
            Fixture.Register<DomainEvent>(() => new BalloonCreated("1", Guid.NewGuid()));
        }

        protected override IEnumerable<Type> ExcludeTypes
        {
            get
            {
                yield return typeof(SagaStateAggregate<>);
                yield return typeof(SagaCreated<>);
                yield return typeof(SagaReceivedMessage<>);
                yield return typeof(CreateNewStateCommand<>);
                yield return typeof(SaveStateCommand<>);
            }
        }

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
        public void Aggregates_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<IAggregate>(AllAssemblies);
        }

        [Fact]
        public void Commands_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<ICommand>(AllAssemblies);
        }

        [Fact]
        public void DomainEvents_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<DomainEvent>(AllAssemblies);
        }

        [Fact]
        public void Saga_commands_aggregates_and_events_should_be_deserializable()
        {
            var state = Fixture.Create<SoftwareProgrammingState>();
            var aggregate = new SagaStateAggregate<SoftwareProgrammingState>(state);
            aggregate.PersistAll();
            Checker.AfterRestore = o =>
                                   {
                                       //little hack because version will be deserialized 
                                       //and increased from produced events
                                       ((IMemento) o).Version = 0;
                                       ((Aggregate) o).PersistAll();
                                   };
            CheckResults(Checker.IsRestorable(aggregate, out string difference1) ?
                             RestoreResult.Ok(aggregate.GetType()) :
                             RestoreResult.Diff(aggregate.GetType(), difference1));
        }

        [Fact]
        public void MessageMetadata_classes_should_be_deserializable()
        {
            CheckAll<object>(typeof(MessageMetadata));
        }

        [Fact]
        public void Scheduler_job_types_from_all_assemblies_should_be_deserializable()
        {
            CheckAll<object>(typeof(ExecutionOptions), typeof(ExecutionOptions), typeof(ScheduleKey));
        }
    }
}