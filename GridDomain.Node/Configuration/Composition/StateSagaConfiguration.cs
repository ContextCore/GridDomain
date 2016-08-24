using System;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class StateSagaConfiguration<TSaga,TSagaState, TStartMessage, TSagaFactory>:
                                             IContainerConfiguration where TSaga : ISagaInstance 
                                             where TSagaFactory : ISagaFactory<TSaga, TSagaState>, 
                                                                  ISagaFactory<TSaga, TStartMessage>,
                                                                  ISagaFactory<TSaga, Guid>
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<TSaga, TSagaState>, TSagaFactory>();
            container.RegisterType<ISagaFactory<TSaga,Guid>, TSagaFactory>();
            container.RegisterType<TSagaFactory>();
            container.RegisterType<ISagaFactory<TSaga, object>>(
                new InjectionFactory(c => new SagaFactoryAdapter<TSaga, TStartMessage>(c.Resolve<TSagaFactory>())));


        }
    }
}