using System;
using System.Linq;
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

            container.RegisterInstance<Type[]>(nameof(TSaga),new[] {typeof(TStartMessage)});
        }
    }

    public class StateSagaConfiguration<TSaga, TSagaState, TSagaFactory> :
                                         IContainerConfiguration where TSaga : ISagaInstance
                                         where TSagaFactory : ISagaFactory<TSaga, TSagaState>,
                                                              ISagaFactory<TSaga, object>,
                                                              ISagaFactory<TSaga, Guid>
    {
        private readonly ISagaDescriptor _descriptor;

        public StateSagaConfiguration(ISagaDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<TSaga, TSagaState>, TSagaFactory>();
            container.RegisterType<ISagaFactory<TSaga, Guid>, TSagaFactory>();
            container.RegisterType<ISagaFactory<TSaga, object>, TSagaFactory>();
            container.RegisterInstance<Type[]>(nameof(TSaga), _descriptor.StartMessages.ToArray());
        }
    }
}