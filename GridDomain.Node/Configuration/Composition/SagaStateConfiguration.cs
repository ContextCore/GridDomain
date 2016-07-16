using GridDomain.EventSourcing.Sagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class SagaStateConfiguration<TSaga,TSagaState, TStartMessage, TSagaFactory>:
        IContainerConfiguration where TSaga : ISagaInstance 
        where TSagaFactory : ISagaFactory<TSaga, TSagaState>, 
            ISagaFactory<TSaga, TStartMessage>, 
            IEmptySagaFactory<TSaga>
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<TSaga, TSagaState>, TSagaFactory>();
            container.RegisterType<ISagaFactory<TSaga, TStartMessage>, TSagaFactory>();
            container.RegisterType<IEmptySagaFactory<TSaga>, TSagaFactory>();
        }
    }
}