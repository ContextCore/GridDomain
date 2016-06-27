using GridDomain.EventSourcing.Sagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class SagaConfiguration<TSaga,TSagaState, TestSagaStartMessage, TSagaFactory>:
        IContainerConfiguration where TSaga : IDomainSaga 
        where TSagaFactory : ISagaFactory<TSaga, TSagaState>, 
            ISagaFactory<TSaga, TestSagaStartMessage>, 
            IEmptySagaFactory<TSaga>
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<TSaga, TSagaState>, TSagaFactory>();
            container.RegisterType<ISagaFactory<TSaga, TestSagaStartMessage>, TSagaFactory>();
            container.RegisterType<IEmptySagaFactory<TSaga>, TSagaFactory>();
        }
    }
}