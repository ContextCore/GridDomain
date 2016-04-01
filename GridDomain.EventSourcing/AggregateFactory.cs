using System;
using System.Reflection;
using CommonDomain;
using CommonDomain.Persistence;

namespace GridDomain.EventSourcing
{
    // By convention, I want to provide two means for creating domain objects. To the public, I want
    // to provide an always-valid constructor. This explicitly shows what needs to be provided to the domain
    // to create a valid instance of that object (eg, Person needs a twitter handle to be valid if I were doing twitter stream analysis)
    // Internally, to EventStore, I want it to be able to create my object via a private ctor and I'm going to pass in the
    // objects id.
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var constructor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (Guid)}, null);

            if (constructor == null)
                throw new ConventionBasedConstructorNotFound();

            return constructor.Invoke(new object[] {id}) as IAggregate;
        }

        public T Build<T>(Guid id, IMemento snapshot = null) where T : IAggregate
        {
            return (T) Build(typeof (T), id, snapshot);
        }
    }
}