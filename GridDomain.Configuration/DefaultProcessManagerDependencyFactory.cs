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

        public ProcessStateDependencyFactory<TState> StateDependencyFactory { get; } = new ProcessStateDependencyFactory<TState>();

        public DefaultProcessDependencyFactory(IProcessDescriptor descriptor, 
                                               Func<IProcess<TState>> processFactory,
                                               Func<IProcessStateFactory<TState>> processStateFactory)
        {
            RouteMapCreator = () => MessageRouteMap.New(descriptor);
            ProcessName = descriptor.ProcessType.BeautyName();
            ProcessFactory = processFactory;
            ProcessStateFactory = processStateFactory;
        }

        public string ProcessName { get; }

        IAggregateDependencyFactory<ProcessStateAggregate<TState>> IProcessDependencyFactory<TState>.StateDependencyFactory => StateDependencyFactory;
        public virtual IMessageRouteMap CreateRouteMap()
        {
            return RouteMapCreator();
        }
    }
}