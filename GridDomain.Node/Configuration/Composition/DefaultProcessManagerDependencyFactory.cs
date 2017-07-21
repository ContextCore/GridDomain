using System;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.ProcessManagers.State;
using GridDomain.Routing;

namespace GridDomain.Node.Configuration.Composition {
    public class DefaultProcessManagerDependencyFactory<TState> : IProcessManagerDependencyFactory<TState>
        where TState : class, IProcessState
    {
        private readonly IProcessManagerCreatorCatalog<TState> _processManager—reatorCatalog;
        public Func<IMessageRouteMap> RouteMapCreator { get; set; }
       
        public DefaultProcessManagerDependencyFactory(IProcessManagerCreatorCatalog<TState> catalog, string processName)
        {
            _processManager—reatorCatalog = catalog;
            ProcessName = processName;
        }
        public DefaultProcessManagerDependencyFactory(IProcessManagerCreator<TState> creator, IProcessManagerDescriptor descriptor):this(BuildCatalog(creator,descriptor),descriptor.ProcessType.BeautyName())
        {
            RouteMapCreator = () => MessageRouteMap.New(descriptor);
        }

        public IProcessManagerCreatorCatalog<TState> CreateCatalog()
        {
            return _processManager—reatorCatalog;
        }

        private static ProcessManager—reatorsCatalog<TState> BuildCatalog(IProcessManagerCreator<TState> factoryCreator, IProcessManagerDescriptor descriptor)
        {
            var producer = new ProcessManager—reatorsCatalog<TState>(descriptor, factoryCreator);
            producer.RegisterAll(factoryCreator);
            return producer;
        }
     
        public virtual ProcessStateDependencyFactory<TState> StateDependencyFactory { get; } = new ProcessStateDependencyFactory<TState>();
        public string ProcessName { get; }

        IAggregateDependencyFactory<ProcessStateAggregate<TState>> IProcessManagerDependencyFactory<TState>.StateDependencyFactory => StateDependencyFactory;
        public virtual IMessageRouteMap CreateRouteMap()
        {
            return RouteMapCreator();
        }
    }
}