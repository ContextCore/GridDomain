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
    public class Types_should_be_deserializable : TypesDeserializationTest
    {
        private readonly Assembly[] _allAssemblies = {
            Assembly.GetAssembly(typeof(GridDomainNode)),
            Assembly.GetAssembly(typeof(SchedulerConfiguration)),
            Assembly.GetAssembly(typeof(SchedulerConfiguration)),
            Assembly.GetAssembly(typeof(SampleAggregate)),
            Assembly.GetAssembly(typeof(ISagaProducer<>)),
            Assembly.GetAssembly(typeof(DomainEvent))
        };

        [Test]
        public void Aggregates_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<IAggregate>(_allAssemblies);
        }

        [Test]
        public void DomainEvents_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<DomainEvent>(_allAssemblies);
        }

        [Test]
        public void Commands_from_all_assemblies_should_be_deserializable()
        {
            CheckAllChildrenOf<ICommand>(_allAssemblies);
        }
    }
}