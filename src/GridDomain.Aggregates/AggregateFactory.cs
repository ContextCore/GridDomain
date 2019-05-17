using System;
using System.Linq;
using System.Reflection;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Aggregates
{
    public class AggregateFactory : IAggregateFactory
    {
        protected static IAggregate BuildByConvention(Type type, string id)
        {
            //TODO: add type cache to reduce search time
            var constructor = type.GetTypeInfo()
                .DeclaredConstructors.FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 0; // && parameters[0]
                    //.ParameterType == typeof(string);
                });

            if (constructor == null)
                throw new ConventionBasedConstructorNotFound();

            var aggregate = (IAggregate) constructor.Invoke(new object[] {});
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