using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.InstanceSagas.Events
{
    public class GotTiredDomainEvent: DomainEvent
    {
        public Guid PersonId => SourceId;
        public Guid LovelySofaId { get; }
        public Guid FavoriteCoffeMachineId { get; }
        public GotTiredDomainEvent(Guid sourceId, Guid lovelySofaId = default(Guid), Guid favoriteCoffeMachineId=default(Guid), DateTime? createdTime = null) : base(sourceId, createdTime)
        {
            LovelySofaId = lovelySofaId;
            FavoriteCoffeMachineId = favoriteCoffeMachineId;
        }
    }
}