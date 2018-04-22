using System;
using Akka.Actor;
using Autofac;
using Autofac.Core;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Configuration.Composition
{
    public class ProcessManagerConfiguration<TState,TActor> : IContainerConfiguration where TState : class, IProcessState
    {
        private readonly IProcessDependencyFactory<TState> _processDependencyFactory;
        private readonly string _statePath;

        public ProcessManagerConfiguration(IProcessDependencyFactory<TState> factory, string statePath)
        {
            _statePath = statePath;
            _processDependencyFactory = factory;
        }

        public void Register(ContainerBuilder container)
        {
            IProcess<TState> process = _processDependencyFactory.CreateProcess();
            container.RegisterInstance<IProcessStateFactory<TState>>(_processDependencyFactory.CreateStateFactory());
            container.RegisterInstance<IProcess<TState>>(process);

            
            container.RegisterType<TActor>()
                     .WithParameters(new Parameter[] {
                                                         new TypedParameter(typeof(IProcess<TState>),  _processDependencyFactory.CreateProcess()),
                                                         new TypedParameter(typeof(IProcessStateFactory<TState>),  _processDependencyFactory.CreateStateFactory()),
                                                         new TypedParameter(typeof(string),  _statePath),
                                                         
                                                     });

            var persistentChildsRecycleConfiguration = _processDependencyFactory.StateDependencyFactory.CreateRecycleConfiguration();
            container.Register<ProcessHubActor<TState>>(c => new ProcessHubActor<TState>(persistentChildsRecycleConfiguration, _processDependencyFactory.ProcessName));


        }
    }
}