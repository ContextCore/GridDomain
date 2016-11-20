using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;
using Quartz.Simpl;

namespace GridDomain.Node.Configuration.Composition
{

    public class SagaConfiguration
    {
        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData>
            (SagaProducer<ISagaInstance<TSaga, TData>> producer, Func<ISnapshotsSavePolicy> snapShotsPolicy = null) where TData : ISagaState
        {
            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer,snapShotsPolicy);
        }



        public static SagaConfiguration<ISagaInstance<TSaga,TData>, SagaDataAggregate<TData>> Instance<TSaga, TData, TFactory, TStartMessage>(ISagaDescriptor descriptor, Func<ISnapshotsSavePolicy> snapShotsPolicy = null)
         where TSaga : Saga<TData>
         where TData : class, ISagaState
         where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                          ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>, new()
        {
            return State<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>, TFactory, TStartMessage>(descriptor,snapShotsPolicy);
        }

        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData, TFactory, TStartMessageA, TStartMessageB, TStartMessageC>(ISagaDescriptor descriptor, Func<ISnapshotsSavePolicy> snapShotsPolicy = null)
               where TSaga : Saga<TData>
               where TData : class, ISagaState
               where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageC>,
                               new()
        {
            var factory = new TFactory();
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            producer.Register<TStartMessageA>(factory);
            producer.Register<TStartMessageB>(factory);
            producer.Register<TStartMessageC>(factory);
            producer.Register<SagaDataAggregate<TData>>(factory);

            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer,snapShotsPolicy);

        }

        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData, TFactory, TStartMessageA, TStartMessageB>
            (ISagaDescriptor descriptor, Func<ISnapshotsSavePolicy> snapShotsPolicy = null, Func<IMemento, SagaDataAggregate<TData>> stateConstructor = null)
               where TSaga : Saga<TData>
               where TData : class, ISagaState
               where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                               new()
        {
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            var factory = new TFactory();

            producer.Register<TStartMessageA>(factory);
            producer.Register<TStartMessageB>(factory);
            producer.Register<SagaDataAggregate<TData>>(factory);
            stateConstructor = stateConstructor ?? SagaDataAggregate<TData>.FromSnapshot;

            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer,snapShotsPolicy, stateConstructor);
        }


        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData>(Func<object, ISagaInstance<TSaga, TData>> factory, ISagaDescriptor descriptor, Func<ISnapshotsSavePolicy> snapShotsPolicy = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
        {
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor, factory);


            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer, snapShotsPolicy);
        }

        public static SagaConfiguration<TSaga, TState> State<TSaga, TState, TFactory, TStartMessage>(ISagaDescriptor descriptor,
                                                                                                     Func<ISnapshotsSavePolicy> snapShotsPolicy = null,
                                                                                                     Func<IMemento,TState> snapshotsConstructor = null )
                where TFactory : ISagaFactory<TSaga, TState>,
                                 ISagaFactory<TSaga, TStartMessage>,
                                 new()

                where TSaga : class, ISagaInstance
                where TState : AggregateBase
        {
            var producer = new SagaProducer<TSaga>(descriptor);
            var factory = new TFactory();
            producer.Register<TState>(factory);
            producer.Register<TStartMessage>(factory);

            return new SagaConfiguration<TSaga, TState>(producer,snapShotsPolicy, snapshotsConstructor);
        }

    }

    public class SagaConfiguration<TSaga,TState> : IContainerConfiguration where TSaga : class, ISagaInstance where TState : AggregateBase
    {
        private readonly SagaProducer<TSaga> _producer;
        private readonly Func<ISnapshotsSavePolicy> _snapshotsPolicyFactory;
        private readonly IConstructAggregates _factory;

        public SagaConfiguration(SagaProducer<TSaga> producer, Func<ISnapshotsSavePolicy> snapShotsPolicy, Func<IMemento, TState> stateProducer)
            :this(producer,snapShotsPolicy,new AggregateSnapshottingFactory<TState>(stateProducer))
        {
        }

        public SagaConfiguration(SagaProducer<TSaga> producer, Func<ISnapshotsSavePolicy> snapShotsPolicy = null, IConstructAggregates factory = null)
        {
            _factory = factory ??  new AggregateFactory();
            _snapshotsPolicyFactory = snapShotsPolicy ?? (() => new NoSnapshotsSavePolicy());
            _producer = producer;
        }
        
        public void Register(IUnityContainer container)
        {
            container.RegisterInstance<ISagaProducer<TSaga>>(_producer);
            container.RegisterInstance(_producer);

            var snapshotsPolicyRegistrationName = typeof(TSaga).Name;

            container.RegisterType<ISnapshotsSavePolicy>(snapshotsPolicyRegistrationName, new InjectionFactory(c => _snapshotsPolicyFactory()));
            container.RegisterType<SagaActor<TSaga, TState>>(
                new InjectionConstructor(new ResolvedParameter<ISagaProducer<TSaga>>(),
                                         new ResolvedParameter<IPublisher>(), 
                                         new ResolvedParameter<ISnapshotsSavePolicy>(snapshotsPolicyRegistrationName),
                                         _factory));

            container.RegisterInstance(snapshotsPolicyRegistrationName, _factory);
        }



    }
}