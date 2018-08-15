using System;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Configuration {
    public class DefaultProcessDependencyFactory<TState> : IProcessDependencyFactory<TState>
        where TState : class, IProcessState
    {
        public IRecycleConfiguration RecycleConfiguration { get; set; }
        private Func<IMessageRouteMap> RouteMapCreator { get; set; }
        private Func<IProcessStateFactory<TState>> ProcessStateFactory { get; set; }
        private Func<IProcess<TState>> ProcessFactory { get; set; }
        public IProcessStateFactory<TState> CreateStateFactory()
        {
            return ProcessStateFactory();
        }

        public IProcess<TState> CreateProcess()
        {
            return ProcessFactory();
        }

        public IRecycleConfiguration CreateRecycleConfiguration()
        {
            return RecycleConfiguration;
        }

        public ProcessStateDependencies<TState> StateDependencies { get; } = new ProcessStateDependencies<TState>();

        public DefaultProcessDependencyFactory(IProcessDescriptor descriptor, 
                                               Func<IProcess<TState>> processFactory,
                                               Func<IProcessStateFactory<TState>> processStateFactory,
                                               IRecycleConfiguration configuration)
        {
            RecycleConfiguration = configuration;
            RouteMapCreator = () => MessageRouteMap.New(descriptor);
            ProcessName = descriptor.ProcessType.BeautyName();
            ProcessFactory = processFactory;
            ProcessStateFactory = processStateFactory;
        }

        public string ProcessName { get; }

        IAggregateDependencies<ProcessStateAggregate<TState>> IProcessDependencyFactory<TState>.StateDependencies => StateDependencies;
        public virtual IMessageRouteMap CreateRouteMap()
        {
            return RouteMapCreator();
        }
    }
}