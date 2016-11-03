using System.Reflection;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Scheduling;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    [Ignore("Under development")]
    public class Aggregates_should_be_deserializable : TypesDeserializationTest
    {
        [Test]
        public void Aggreagtes_from_all_assemblies_should_be_deserializable()
        {
            base.CheckAllChildrenOf<IAggregate>(
                Assembly.GetAssembly(typeof(GridDomainNode)),
                Assembly.GetAssembly(typeof(SchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SampleAggregate)),
                Assembly.GetAssembly(typeof(ISagaProducer<>)),
                Assembly.GetAssembly(typeof(DomainEvent))
                );
        }

        [Test]
        public void DomainEvents_from_all_assemblies_should_be_deserializable()
        {
            base.CheckAllChildrenOf<DomainEvent>(
                Assembly.GetAssembly(typeof(GridDomainNode)),
                Assembly.GetAssembly(typeof(SchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SampleAggregate)),
                Assembly.GetAssembly(typeof(ISagaProducer<>)),
                Assembly.GetAssembly(typeof(DomainEvent))
                );
        }

        [Test]
        public void Commands_from_all_assemblies_should_be_deserializable()
        {
            base.CheckAllChildrenOf<ICommand>(
                Assembly.GetAssembly(typeof(GridDomainNode)),
                Assembly.GetAssembly(typeof(SchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SchedulerConfiguration)),
                Assembly.GetAssembly(typeof(SampleAggregate)),
                Assembly.GetAssembly(typeof(ISagaProducer<>)),
                Assembly.GetAssembly(typeof(DomainEvent))
                );
        }
    }
}