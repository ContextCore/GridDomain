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
    internal class ProcessManagerConfiguration<TState> : IContainerConfiguration where TState : class, IProcessState
    {
        private readonly IProcessDependencyFactory<TState> _processDependencyFactory;

        internal ProcessManagerConfiguration(IProcessDependencyFactory<TState> factory)
        {
            _processDependencyFactory = factory;
        }

        //private void RegisterStateAggregate<TStateActorType>(ContainerBuilder container)
        //{
        //    container.Register(new AggregateConfiguration<TStateActorType, ProcessStateAggregate<TState>>(CommandAggregateHandler.New<ProcessStateAggregate<TState>>(),
        //                                                                                                  _processDependencyFactory.StateDependencyFactory.CreatePersistencePolicy,
        //                                                                                                  _processDependencyFactory.StateDependencyFactory.CreateAggregateFactory(),
        //                                                                                                  _processDependencyFactory.StateDependencyFactory.CreateRecycleConfiguration()));
        //}

        public void Register(ContainerBuilder container)
        {
            IProcess<TState> process = _processDependencyFactory.CreateProcess();
            container.RegisterInstance<IProcessStateFactory<TState>>(_processDependencyFactory.CreateStateFactory());
            container.RegisterInstance<IProcess<TState>>(process);
            container.RegisterType<ProcessActor<TState>>();

            container.RegisterType<ProcessActor<TState>>()
                     .WithParameters(new Parameter[] {
                                                         new TypedParameter(typeof(IProcess<TState>),  _processDependencyFactory.CreateProcess()),
                                                         new TypedParameter(typeof(IProcessStateFactory<TState>),  _processDependencyFactory.CreateStateFactory()),
                                                         new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IActorRef),
                                                                               (pi, ctx) => ctx.ResolveNamed<IActorRef>(_processDependencyFactory.ProcessName))
                                                     });

            var persistentChildsRecycleConfiguration = _processDependencyFactory.StateDependencyFactory.CreateRecycleConfiguration();
            container.Register<ProcessHubActor<TState>>(c => new ProcessHubActor<TState>(persistentChildsRecycleConfiguration, _processDependencyFactory.ProcessName));


            //for direct access to process state from repositories and for generalization
           // RegisterStateAggregate<AggregateActor<ProcessStateAggregate<TState>>>(container);
           // container.Register<AggregateHubActor<ProcessStateAggregate<TState>>>(c => new AggregateHubActor<ProcessStateAggregate<TState>>(persistentChildsRecycleConfiguration));
        }
    }
}