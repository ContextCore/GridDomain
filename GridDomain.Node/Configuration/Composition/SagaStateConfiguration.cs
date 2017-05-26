using System;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.Configuration.Composition
{
    public class SagaStateConfiguration<TState> : AggregateBaseConfiguration<SagaStateActor<TState>, SagaStateAggregate<TState>, SagaStateCommandHandler<TState>>
        where TState : ISagaState
    {
        public SagaStateConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                      Func<IMemento, SagaStateAggregate<TState>> snapshotsFactory = null) : base(snapshotsPolicy, snapshotsFactory) {}
    }
}