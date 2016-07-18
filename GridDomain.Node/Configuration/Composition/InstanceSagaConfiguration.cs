using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class InstanceSagaConfiguration<TSaga, TData, TStartMessage, TSagaFactory> :
        IContainerConfiguration where TSaga : Saga<TData>
        where TSagaFactory : ISagaFactory<ISagaInstance<TSaga,TData>, SagaDataAggregate<TData>>,
                             ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>,
                             ISagaFactory<ISagaInstance<TSaga, TData>, Guid>
        where TData : class, ISagaState<State>
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>, TSagaFactory>();
            container.RegisterType<ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>, TSagaFactory>();
            container.RegisterType<ISagaFactory<ISagaInstance<TSaga, TData>, Guid>, TSagaFactory>();
        }
    }
}