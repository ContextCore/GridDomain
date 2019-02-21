using System;
using System.Linq;
using System.Reflection;

namespace GridDomain.Aggregates
{
    /// <summary>
    /// By convention, I want to provide two means for creating domain objects. To the public, I want
    /// to provide an always-valid constructor. This explicitly shows what needs to be provided to the domain
    /// to create a valid instance of that object (eg, Person needs a twitter handle to be valid if I were doing twitter stream analysis)
    /// Internally, to EventStore, I want it to be able to create my object via a private ctor and I'm going to pass in the
    /// objects id.
    /// </summary>
    public class AggregateFactory : IAggregateFactory
    {
        protected static IAggregate BuildByConvention(Type type, string id)
        {
            //TODO: add type cache to reduce search time
            var constructor = type.GetTypeInfo()
                .DeclaredConstructors.FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 && parameters[0]
                               .ParameterType == typeof(string);
                });

            if (constructor == null)
                throw new ConventionBasedConstructorNotFound();

            var aggregate = (IAggregate) constructor.Invoke(new object[] {id});
            return aggregate;
        }

        public virtual IAggregate Build(Type type, string id)
        {
            return BuildByConvention(type, id);
        }

        public static readonly AggregateFactory Default = new AggregateFactory();

        public static AggregateFactory<T> For<T>() where T : IAggregate => new AggregateFactory<T>();
    }

    public class AggregateFactory<T> : AggregateFactory, IAggregateFactory<T> where T : IAggregate
    {
        public T Build(string id = null)
        {
            return (T) Build(typeof(T), id ?? Guid.NewGuid().ToString());
        }
    }
}