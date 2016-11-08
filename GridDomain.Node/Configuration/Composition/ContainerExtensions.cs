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

        [Obsolete("Use create configuration via SagaConfiguration.Instance and register in container instead")]
        public static void RegisterSaga<TSaga, TData,TFactory, TStartMessage>(this IUnityContainer container, ISagaDescriptor descriptor)
           where TSaga : Saga<TData>
           where TData : class, ISagaState
           where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                            ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage> ,new()
        {
            SagaConfiguration.Instance<TSaga, TData, TFactory, TStartMessage>(descriptor)
                             .Register(container);
        }


        public static void RegisterSaga<TSaga, TData, TFactory, TStartMessageA, TStartMessageB>(this IUnityContainer container, ISagaDescriptor descriptor)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
                 where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                  ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                  ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                                 new()
        {
            SagaConfiguration.Instance<TSaga, TData, TFactory, TStartMessageA, TStartMessageB>(descriptor)
                             .Register(container);
        }

        [Obsolete("Use create configuration via SagaConfiguration.Instance and register in container instead")]
        public static void RegisterSaga<TSaga, TData, TFactory, TStartMessageA, TStartMessageB, TStartMessageC>(this IUnityContainer container, ISagaDescriptor descriptor)
               where TSaga : Saga<TData>
               where TData : class, ISagaState
               where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageC>,
                               new()
        {
            SagaConfiguration.Instance<TSaga, TData, TFactory, TStartMessageA, TStartMessageB, TStartMessageC>(descriptor)
                             .Register(container);
        }


        [Obsolete("Use create configuration via SagaConfiguration.Instance and register in container instead")]


        public static void RegisterSaga<TSaga, TData>(this IUnityContainer container,  Func<object, ISagaInstance<TSaga, TData>> factory, ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
        {
            SagaConfiguration.Instance<TSaga, TData>(factory,descriptor)
                             .Register(container);
        }

        public static void RegisterStateSaga<TSaga, TState, TFactory, TStartMessage>(this IUnityContainer container,ISagaDescriptor descriptor)
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

            var conf = new SagaConfiguration<TSaga, TState>(producer);
            container.Register(conf);
        }
    }
}