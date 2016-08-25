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
        public InstanceSagaConfiguration(ISagaDescriptor<ISagaInstance<TSaga, TData>> sagaDescriptor) : base(sagaDescriptor)
        {
        }
    }

    public class InstanceSagaConfiguration<TSaga, TData, TStartMessage, TSagaFactory> :
       StateSagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>, TStartMessage, TSagaFactory>
       where TSagaFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                            ISagaFactory<ISagaInstance<TSaga, TData>, object>,
                            ISagaFactory<ISagaInstance<TSaga, TData>, Guid>
       where TData : class, ISagaState
    {
        public InstanceSagaConfiguration(ISagaDescriptor<ISagaInstance<TSaga, TData>> sagaDescriptor) : base(sagaDescriptor)
        {
        }
    }
}