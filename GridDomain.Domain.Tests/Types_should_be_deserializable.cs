using System;
using System.Collections.Generic;
using System.Reflection;
using CommonDomain;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.SampleDomain;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using SoftwareProgrammingSaga = GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga;

namespace GridDomain.Tests
{
    [TestFixture]
    public class Types_should_be_deserializable : TypesDeserializationTest
    {
        protected override IEnumerable<Type> ExcludeTypes
        {
            get
            {
                yield return typeof(SagaDataAggregate<>);
                yield return typeof(SagaStateAggregate<,>);
                yield return typeof(SagaCreatedEvent<>);
                yield return typeof(SagaMessageReceivedEvent<>);
                yield return typeof(SagaTransitionEvent<>);
                yield return typeof(SagaTransitionEvent<,>);
            }
        }

        public Types_should_be_deserializable()
        {
            Fixture.Register<ICommand>(() => new FakeCommand());
            Fixture.Register<Command>(() => new FakeCommand());
        }

        class FakeCommand : Command
        {
            
        }
        [Test]
        public void Generic_domain_classes_should_be_deserializable()
        {
            CheckAll<object>(typeof(SagaDataAggregate<SoftwareProgrammingSagaData>),
                             typeof(SagaStateAggregate<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>),
                             typeof(SagaCreatedEvent<SoftwareProgrammingSagaData>),
                             typeof(SagaTransitionEvent<SoftwareProgrammingSagaData>),
                             typeof(SagaTransitionEvent<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>)
                            );
        }
        [Test]
        public void Saga_events_classes_should_be_deserializable()
        {
            CheckAll<object>(typeof(SagaTransitionEvent<SoftwareProgrammingSagaData>));
        }

        [Test]
        public void MessageMetadata_classes_should_be_deserializable()
        {
            CheckAll<object>(typeof(MessageMetadata));
        }

        [Test]
        public void Scheduler_job_types_from_all_assemblies_should_be_deserializable()
        {
            CheckAll<object>(typeof(ExecutionOptions),
                             typeof(ExtendedExecutionOptions),
                             typeof(ScheduleKey));
        }
       
        [Test]
        public void Aggregates_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<IAggregate>(AllAssemblies);
        }

        [Test]
        public void DomainEvents_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<DomainEvent>(AllAssemblies);
        }

        [Test]
        public void Commands_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<ICommand>(AllAssemblies);
        }

        protected override Assembly[] AllAssemblies { get; } =
            {
                Assembly.GetAssembly(typeof(GridDomainNode)),
                Assembly.GetAssembly(typeof(QuartzSchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SagaTransitionEvent<>)),
                Assembly.GetAssembly(typeof(SagaMessageReceivedEvent<>)),
                Assembly.GetAssembly(typeof(SampleAggregate)),
                Assembly.GetAssembly(typeof(ISagaProducer<>)),
                Assembly.GetAssembly(typeof(DomainEvent)),
                Assembly.GetAssembly(typeof(ExecutionOptions))
            };
    }
}