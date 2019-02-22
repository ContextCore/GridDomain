using Akka.Actor;
using Autofac;
using GridDomain.Aggregates;

namespace GridDomain.Scheduling.Akka {

    
    public class AggregatesExtension : IExtension
    {
        private readonly IContainer _container;

        public AggregatesExtension(IContainer container)
        {
            _container = container;
        }

        public IAggregateDependencies<T> GetDependencies<T>() where T : IAggregate
        {
            return _container.Resolve<IAggregateDependencies<T>>();
        }
    }
}