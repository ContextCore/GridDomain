using System;

using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.Configuration.Composition
{
  

    public class AggregateConfiguration<TAggregate, TAggregateCommandsHandler> : AggregateBaseConfiguration<AggregateActor<TAggregate>, TAggregate, TAggregateCommandsHandler>
        where TAggregate : Aggregate
        where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
    {
        public AggregateConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                      Func<IMemento, TAggregate> snapshotsFactory = null) : base(snapshotsPolicy, snapshotsFactory) {}
    }
}