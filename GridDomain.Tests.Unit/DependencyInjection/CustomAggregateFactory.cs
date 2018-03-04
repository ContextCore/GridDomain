using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.CommandsExecution;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.DependencyInjection
{

    public class CustomAggregateFactoryTests: NodeTestKit
    {
        public CustomAggregateFactoryTests(ITestOutputHelper output):base(
            new NodeTestFixture(output, new CustomAggregateFactoryDomain()))
        {

        }

        public class CustomAggregateFactoryDomain:IDomainConfiguration
        {
            public void Register(IDomainBuilder builder)
            {
                var dep = new Dependency(10);
                var factory = DefaultAggregateDependencyFactory.ForCommandAggregate<AggregateWithDependency>(new CustomAggregateFactory(dep));
                builder.RegisterAggregate(factory);
            }
        }

        class Dependency : IDependency
        {
            private readonly int _value;

            public Dependency(int value)
            {
                _value = value;
            }
            public int GetValue()
            {
                return _value;
            }
        }
        interface IDependency
        {
            int GetValue();
        }

        class CustomAggregateFactory : AggregateFactory
        {
            private readonly IDependency _dependency;

            public CustomAggregateFactory(IDependency dep)
            {
                _dependency = dep;
            }
            public override IAggregate Build(Type type, string id, IMemento snapshot)
            {
                if (type == typeof(AggregateWithDependency))
                {
                    return new AggregateWithDependency(id,_dependency);
                }
                return base.Build(type, id, snapshot);
            }
        }

        class CreateCommand : Command
        {
            public int Value { get; }

            public CreateCommand(string id, int value) : base(id)
            {
                Value = value;
            }
            public CreateCommand(Guid id, int value) : base(id.ToString())
            {
                Value = value;
            }
        }

        class AggregateWithDependency : ConventionAggregate
        {
            internal AggregateWithDependency(string id, IDependency dep):base(id)
            {
                Apply<Created>(c => { });
                Execute<CreateCommand>(c => new AggregateWithDependency(c.AggregateId, c.Value,dep));
            }

            public AggregateWithDependency(string id, int value, IDependency dep):this(id,dep)
            {
                Emit(new[] {new Created(id,value,dep.GetValue())});
            }
        }

        [Fact]
        public async Task AggregateWithCustom_AggregateFactory_should_execute_commands()
        {
            var evt = await Node.Prepare(new CreateCommand(Guid.NewGuid(), 20))
                          .Expect<Created>()
                          .Execute();

            Assert.Equal(10, evt.Received.Produced); //from injected dependency
        }
    }

    internal class Created : DomainEvent
    {
        public int InitialValue { get; }
        public int Produced { get; }

        public Created(string id, int initialValue, int produced):base(id)
        {
            InitialValue = initialValue;
            Produced = produced;
        }
    }
}
