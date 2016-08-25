using System;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
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

        public static void RegisterSaga<TSaga, TData, TStartMessage,TFactory>(this IUnityContainer container)
           where TSaga : Saga<TData>
           where TData : class, ISagaState
           where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                            ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>,
                            ISagaFactory<ISagaInstance<TSaga, TData>, Guid>, new()
        {
            RegisterStateSaga<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>, TStartMessage, TFactory>(container);
        }

        public static void RegisterStateSaga<TSaga, TState, TStartMessage, TFactory>(this IUnityContainer container)
                where TFactory : ISagaFactory<TSaga, TState>, 
                                 ISagaFactory<TSaga, TStartMessage>, 
                                 ISagaFactory<TSaga, Guid>, new()

                where TSaga : ISagaInstance
        {
            var factory = new TFactory();
            var conf = SagaConfiguration<TSaga>.New<TFactory,TState>(factory);
            conf.Register<TStartMessage>(factory);
            conf.Register(container);
        }


    }
}