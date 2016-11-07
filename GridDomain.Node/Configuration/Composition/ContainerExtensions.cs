using System;
using System.Linq;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public static class ContainerExtensions
    {
        public static void Register(this IUnityContainer container, IContainerConfiguration configuration)
        {
            configuration.Register(container);
        }

        public static void Register<TConfiguration>(this IUnityContainer container) where TConfiguration: IContainerConfiguration, new()
        {
            new TConfiguration().Register(container);
        }

        public static void RegisterAggregate<TAggregate, TCommandsHandler>(this IUnityContainer container) where TCommandsHandler : ICommandAggregateLocator<TAggregate>, IAggregateCommandsHandler<TAggregate> where TAggregate : AggregateBase
        {
            Register<AggregateConfiguration<TAggregate, TCommandsHandler>>(container);
        }

        public static void RegisterSaga<TSaga, TData,TFactory, TStartMessage>(this IUnityContainer container, ISagaDescriptor descriptor)
           where TSaga : Saga<TData>
           where TData : class, ISagaState
           where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                            ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage> ,new()
        {
            RegisterStateSaga<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>, TFactory, TStartMessage>(container, descriptor);
        }

        public static void RegisterSaga<TSaga, TData, TFactory, TStartMessageA, TStartMessageB, TStartMessageC>(this IUnityContainer container, ISagaDescriptor descriptor)
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

            var conf = new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer);
            
            container.Register(conf);
        }


        public static void RegisterSaga<TSaga, TData, TFactory, TStartMessageA, TStartMessageB>(this IUnityContainer container, ISagaDescriptor descriptor)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
                 where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                  ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                  ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                                 new()
        {
            var factory = new TFactory();
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            producer.Register<TStartMessageA>(factory);
            producer.Register<TStartMessageB>(factory);
            producer.Register<SagaDataAggregate<TData>>(factory);

            var conf = new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer);

            container.Register(conf);
        }


        public static void RegisterSaga<TSaga, TData>(this IUnityContainer container,  Func<object, ISagaInstance<TSaga, TData>> factory, ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
        {
            var producer = new SagaProducer<ISagaInstance<TSaga,TData>>(descriptor);
            foreach (var dataType in descriptor.StartMessages)
                producer.Register(dataType, factory);

            var conf = new SagaConfiguration<ISagaInstance<TSaga, TData>,SagaDataAggregate<TData>>(producer, snapShotsPolicy);
            conf.Register(container);
        }

        public static void RegisterStateSaga<TSaga, TState, TFactory, TStartMessage>(this IUnityContainer container,ISagaDescriptor descriptor)
                where TFactory : ISagaFactory<TSaga, TState>, 
                                 ISagaFactory<TSaga, TStartMessage>, 
                                 new()

                where TSaga : class, ISagaInstance 
                where TState : AggregateBase
        {
            var factory = new TFactory();
            var producer = new SagaProducer<ISagaInstance<TSaga, TState>>(descriptor);
            foreach (var dataType in descriptor.StartMessages)
                producer.Register(dataType, factory);

            var conf = new SagaConfiguration<TSaga, TState>.New<TFactory>(factory, descriptor);
            conf.Register<TStartMessage>(factory);
            container.Register(conf);
        }


    }
}