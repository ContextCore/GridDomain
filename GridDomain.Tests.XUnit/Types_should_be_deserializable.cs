using System;
using System.Collections.Generic;
using System.Reflection;
using CommonDomain;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.SampleDomain;
using Ploeh.AutoFixture;
using Xunit;

namespace GridDomain.Tests.XUnit
{
    
    public class Types_should_be_deserializable : TypesDeserializationTest
    {
        protected override IEnumerable<Type> ExcludeTypes
        {
            get
            {
                yield return typeof(SagaStateAggregate<>);
                yield return typeof(SagaCreatedEvent<>);
                yield return typeof(SagaMessageReceivedEvent<>);
            }
        }

        public Types_should_be_deserializable()
        {
            Fixture.Register<ICommand>(() => new FakeCommand());
            Fixture.Register<Command>(() => new FakeCommand());
        }

        class FakeCommand : Command
        {
            public FakeCommand():base(Guid.NewGuid())
            {
                
            } 
        }
       [Fact]
        public void Generic_domain_classes_should_be_deserializable()
        {
            CheckAll<object>(typeof(SagaStateAggregate<SoftwareProgrammingSagaData>),
                             typeof(SagaCreatedEvent<SoftwareProgrammingSagaData>)
                            );
        }

       [Fact]
        public void MessageMetadata_classes_should_be_deserializable()
        {
            CheckAll<object>(typeof(MessageMetadata));
        }

       [Fact]
        public void Scheduler_job_types_from_all_assemblies_should_be_deserializable()
        {
            CheckAll<object>(typeof(ExecutionOptions),
                             typeof(ExtendedExecutionOptions),
                             typeof(ScheduleKey));
        }
       
       [Fact]
        public void Aggregates_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<IAggregate>(AllAssemblies);
        }

       [Fact]
        public void DomainEvents_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<DomainEvent>(AllAssemblies);
        }

       [Fact]
        public void Commands_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<ICommand>(AllAssemblies);
        }

        protected override Assembly[] AllAssemblies { get; } =
            {
                Assembly.GetAssembly(typeof(GridDomainNode)),
                Assembly.GetAssembly(typeof(QuartzSchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SagaMessageReceivedEvent<>)),
                Assembly.GetAssembly(typeof(SampleAggregate)),
                Assembly.GetAssembly(typeof(ISagaProducer<>)),
                Assembly.GetAssembly(typeof(DomainEvent)),
                Assembly.GetAssembly(typeof(ExecutionOptions))
            };
    }
}