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
        public Func<IMessageRouteMap> RouteMapCreator { get; set; }
        public Func<IProcessStateFactory<TState>> ProcessStateFactory { get; set; }
        public Func<IProcess<TState>> ProcessFactory { get; set; }
        public IProcessStateFactory<TState> CreateStateFactory()
        {
            return ProcessStateFactory();
        }

        public IProcess<TState> CreateProcess()
        {
            return ProcessFactory();
        }

        public virtual ProcessStateDependencyFactory<TState> StateDependencyFactory { get; } = new ProcessStateDependencyFactory<TState>();


        public DefaultProcessDependencyFactory(IProcessDescriptor descriptor)
        {
            RouteMapCreator = () => MessageRouteMap.New(descriptor);
            ProcessName = descriptor.ProcessType.BeautyName();
        }

        public string ProcessName { get; }

        IAggregateDependencyFactory<ProcessStateAggregate<TState>> IProcessDependencyFactory<TState>.StateDependencyFactory => StateDependencyFactory;
        public virtual IMessageRouteMap CreateRouteMap()
        {
            return RouteMapCreator();
        }
    }
}