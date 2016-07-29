using System;
using CommonDomain.Core;
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
            where TSaga: Saga<TData> 
            where TData : class, ISagaState 
            where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                             ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>,
                             ISagaFactory<ISagaInstance<TSaga, TData>, Guid>
        {
            Register<InstanceSagaConfiguration<TSaga, TData, TStartMessage,TFactory>>(container);
        }

        public static void RegisterStateSaga<TSaga, TState, TStartMessage, TFactory>(this IUnityContainer container)
            where TFactory : ISagaFactory<TSaga, TState>, ISagaFactory<TSaga, TStartMessage>, ISagaFactory<TSaga, Guid> 
            where TSaga : ISagaInstance
            //where TSaga : Saga<TState>
            //where TState : class, ISagaState<State>
            //where TFactory : ISagaFactory<ISagaInstance<TSaga, TState>, SagaDataAggregate<TState>>,
                             //ISagaFactory<ISagaInstance<TSaga, TState>, TStartMessage>,
                             //ISagaFactory<ISagaInstance<TSaga, TState>, Guid>
        {
            Register<StateSagaConfiguration<TSaga, TState, TStartMessage, TFactory>>(container);
        }

    }
}