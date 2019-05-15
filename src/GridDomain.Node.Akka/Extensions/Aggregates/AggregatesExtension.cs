using Akka.Actor;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Common;

namespace GridDomain.Node.Akka.Extensions.Aggregates {

    
    public class AggregatesExtension : IExtension
    {
        private readonly IContainer _container;

        public AggregatesExtension(IContainer container)
        {
            _container = container;
        }

        public IAggregateConfiguration<T> GetConfiguration<T>() where T : IAggregate
        {
            return _container.Resolve<IAggregateConfiguration<T>>();
        }

        public ICommandsResultAdapter GetAdapter<T>() where T : IAggregate
        {
            _container.TryResolveNamed(typeof(T).BeautyName(),typeof(ICommandsResultAdapter), out var adapter);
            return (adapter as ICommandsResultAdapter) ?? CommandsResultNullAdapter.Instance;
        }
    }
}