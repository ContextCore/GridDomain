using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using Microsoft.Practices.Unity;
using Quartz.Simpl;

namespace GridDomain.Node.Configuration.Composition
{

    public class SagaConfiguration
    {
        public static IContainerConfiguration Instance<TSaga, TData,TFactory>(ISagaDescriptor descriptor, Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null, IConstructAggregates factory = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
                 where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaStateAggregate<TData>>
        {
            return new SagaConfiguration<TSaga, TData, TFactory>(descriptor,snapShotsPolicy, factory);
        }
    }


    public class SagaConfiguration<TSaga, TState, TFactory> : IContainerConfiguration
        where TSaga : Saga<TState>
        where TState : class, ISagaState
        where TFactory : ISagaFactory<ISagaInstance<TSaga, TState>, SagaStateAggregate<TState>>
    {
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;
        private readonly IConstructAggregates _aggregateFactory;
        private readonly ISagaDescriptor _descriptor;

        public SagaConfiguration(ISagaDescriptor descriptor, Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null, IConstructAggregates factory = null)
        {
            _descriptor = descriptor;
            _aggregateFactory = factory ?? new AggregateFactory();
            _snapShotsPolicy = snapShotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        public void Register(IUnityContainer container)
        {
            var sagaSpecificRegistrationsName = typeof(TSaga).Name;
            container.RegisterInstance<IConstructAggregates>(sagaSpecificRegistrationsName,_aggregateFactory);
            container.RegisterType<ISnapshotsPersistencePolicy>(sagaSpecificRegistrationsName, new InjectionFactory(c => _snapShotsPolicy()));

            container.RegisterType<ISagaProducer<ISagaInstance<TSaga, TState>>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(
                    c =>
                    {
                        var factory = c.Resolve<TFactory>();
                        var producer = new SagaProducer<ISagaInstance<TSaga, TState>>(_descriptor);
                        producer.RegisterAll<TFactory, TState>(factory);
                        return producer;
                    }));

           container.RegisterType<SagaActor<ISagaInstance<TSaga, TState>, SagaStateAggregate<TState>>>(
               new InjectionConstructor(
                        new ResolvedParameter<ISagaProducer<ISagaInstance<TSaga, TState>>>(),
                        new ResolvedParameter<IPublisher>(),
                        new ResolvedParameter<ISnapshotsPersistencePolicy>(sagaSpecificRegistrationsName),
                        new ResolvedParameter<IConstructAggregates>(sagaSpecificRegistrationsName)
                        ));

            container.RegisterType<SagaHubActor<ISagaInstance<TSaga, TState>, SagaStateAggregate<TState>>>(new InjectionConstructor(
                         new ResolvedParameter<IPersistentChildsRecycleConfiguration>(),
                         new ResolvedParameter<ISagaProducer<ISagaInstance<TSaga, TState>>>(),
                         new ResolvedParameter<IActorRef>(SagaProcessActor.SagaProcessActorRegistrationName)));

        }
    }
}