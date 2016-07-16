using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class InstanceSagaConfiguration<TSaga, TData, TStartMessage, TSagaFactory> :
        IContainerConfiguration where TSaga : Saga<TData>
        where TSagaFactory : ISagaFactory<ISagaInstance, SagaDataAggregate<TData>>,
                             ISagaFactory<ISagaInstance, TStartMessage>,
                             IEmptySagaFactory<ISagaInstance>
        where TData : class, ISagaState<State>
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<ISagaInstance, SagaDataAggregate<TData>>, TSagaFactory>();
            container.RegisterType<ISagaFactory<ISagaInstance, TStartMessage>, TSagaFactory>();
            container.RegisterType<IEmptySagaFactory<ISagaInstance>, TSagaFactory>();
        }
    }
}