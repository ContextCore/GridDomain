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
        protected readonly IProcessDependencyFactory<TState> ProcessDependencyFactory;
        private readonly string _statePath;

        public ProcessManagerConfiguration(IProcessDependencyFactory<TState> factory, string statePath)
        {
            _statePath = statePath;
            ProcessDependencyFactory = factory;
        }

        public void Register(ContainerBuilder container)
        {
            IProcess<TState> process = ProcessDependencyFactory.CreateProcess();
            container.RegisterInstance<IProcessStateFactory<TState>>(ProcessDependencyFactory.CreateStateFactory());
            container.RegisterInstance<IProcess<TState>>(process);

            
            container.RegisterType<TActor>()
                     .WithParameters(CreateParametersRegistration());

            var persistentChildsRecycleConfiguration = ProcessDependencyFactory.StateDependencies.CreateRecycleConfiguration();
            container.Register<ProcessHubActor<TState>>(c => new ProcessHubActor<TState>(persistentChildsRecycleConfiguration, ProcessDependencyFactory.ProcessName));


        }

        protected virtual Parameter[] CreateParametersRegistration()
        {
            return new Parameter[] {
                                       new TypedParameter(typeof(IProcess<TState>),  ProcessDependencyFactory.CreateProcess()),
                                       new TypedParameter(typeof(IProcessStateFactory<TState>),  ProcessDependencyFactory.CreateStateFactory()),
                                       new TypedParameter(typeof(string),  _statePath),
                                                         
                                   };
        }
    }
}