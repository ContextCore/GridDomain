using System;
using System.Collections.Generic;
using System.Reflection;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Serialization;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using AutoFixture;
using AutoFixture.Kernel;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Node.Serializers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace GridDomain.Tests.Unit
{
    public class Types_should_be_deserializable : TypesDeserializationTest
    {
        private readonly ExtendedActorSystem _system;
        protected override ObjectDeserializationChecker Checker { get; } 

        public Types_should_be_deserializable()
        {

           Fixture.Customizations.Add(new TypeRelay(typeof(IMemento), typeof(ProgrammerAggregate)));
           Fixture.Customizations.Add(new TypeRelay(typeof(ICommand), typeof(FakeCommand)));
           Fixture.Customizations.Add(new TypeRelay(typeof(Command), typeof(FakeCommand)));
           Fixture.Customizations.Add(new TypeRelay(typeof(DomainEvent), typeof(BalloonCreated)));
           Fixture.Customizations.Add(new TypeRelay(typeof(IProcessState), typeof(SoftwareProgrammingState)));


            _system = (ExtendedActorSystem)TestActorSystem.Create();
            var testActor = _system.ActorOf<BlackHoleActor>();
            _system.InitDomainEventsSerialization(new EventsAdaptersCatalog());
            Fixture.Register<IActorRef>(() => testActor);
            Checker = GetChecker(new DomainEventsJsonAkkaSerializer(_system), new HyperionSerializer(_system));
        }

        private ObjectDeserializationChecker GetChecker(params Serializer[] ser)
        {
            return new ObjectDeserializationChecker(null, ser);
        }
        protected override IEnumerable<Type> ExcludeTypes => new List<Type>();

        private class FakeCommand : Command
        {
            public FakeCommand(string aggregateId) : base(aggregateId,"fake") {}
        }

        protected override Assembly[] AllAssemblies { get; } = {
                                                                   Assembly.GetAssembly(typeof(GridDomainNode)),
                                                                   Assembly.GetAssembly(typeof(SchedulingConfiguration)),
                                                                   Assembly.GetAssembly(typeof(ProcessReceivedMessage<>)),
                                                                   Assembly.GetAssembly(typeof(Balloon)),
                                                                   Assembly.GetAssembly(typeof(IProcessStateFactory<>)),
                                                                   Assembly.GetAssembly(typeof(DomainEvent)),
                                                                   Assembly.GetAssembly(typeof(ExecutionOptions))
                                                               };

        //aggregates are not serializable itself ! 
        [Fact]
        public void IMemento_from_all_assemblies_should_be_deserializable_by_json()
        {
            var checker = GetChecker(new DomainEventsJsonAkkaSerializer(_system));
            CheckAllChildrenOf<IMemento>(checker, AllAssemblies);
        }

        [Fact]
        public void Commands_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {
            CheckAllChildrenOf<ICommand>(Checker, AllAssemblies);
        }

        [Fact]//(Skip = "Postponing rework of all exception to propers support of ISerializable")]
        public void Exceptions_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {

           //Hyperion cares only about limited set of exception fields https://github.com/akkadotnet/Hyperion/blob/f9487dd154bb3cb8344f860a44e3697e8aac6c3a/Hyperion/SerializerFactories/ExceptionSerializerFactory.cs

            var checker = new ObjectDeserializationChecker(new CompareLogic
                                                          {
                                                              Config = new ComparisonConfig() { MembersToInclude = new List<string>
                                                                                                                   {
                                                                                                                       nameof(Exception.Message),
                                                                                                                       nameof(Exception.StackTrace),
                                                                                                                       nameof(Exception.InnerException)
                                                                                                                   } }
                                                          }, 
                                                          new HyperionSerializer(_system));

            CheckAllChildrenOf<Exception>(checker, AllAssemblies);
        }

        [Fact]
        public void DomainEvents_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {
            CheckAllChildrenOf<DomainEvent>(Checker, AllAssemblies);
        }

        [Fact]
        public void ProcessState_from_all_assemblies_should_be_deserializable_by_json_and_wire()
        {
            CheckAllChildrenOf<IProcessState>(Checker, AllAssemblies);
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