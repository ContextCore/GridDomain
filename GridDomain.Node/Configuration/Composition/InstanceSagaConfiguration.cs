using System;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class InstanceSagaConfiguration<TSaga, TData, TStartMessage, TSagaFactory> :
        StateSagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>,TStartMessage,TSagaFactory>
        where TSagaFactory : ISagaFactory<ISagaInstance<TSaga,TData>, SagaDataAggregate<TData>>,
                             ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>,
                             ISagaFactory<ISagaInstance<TSaga, TData>, Guid>
        where TData : class, ISagaState
    {
        //public void Register(IUnityContainer container)
        //{
        //    container.RegisterType<ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>, TSagaFactory>();
        //    container.RegisterType<TSagaFactory>();
        //    container.RegisterType<ISagaFactory<ISagaInstance<TSaga, TData>, object>>(
        //        new InjectionFactory(c => new SagaFactoryAdapter<ISagaInstance<TSaga, TData>, TStartMessage>(c.Resolve<TSagaFactory>())));
            
        //    container.RegisterType<ISagaFactory<ISagaInstance<TSaga, TData>, Guid>, TSagaFactory>();
        //}
    }
}