using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events
{
    public class GotTiredEvent : DomainEvent
    {
        public GotTiredEvent(Guid sourceId,
                             Guid lovelySofaId = default(Guid),
                             Guid favoriteCoffeMachineId = default(Guid),
                             Guid? sagaId = null,
                             DateTime? createdTime = null) : base(sourceId, sagaId, createdTime: createdTime)
        {
            LovelySofaId = lovelySofaId;
            FavoriteCoffeMachineId = favoriteCoffeMachineId;
        }

        public Guid PersonId => SourceId;
        public Guid LovelySofaId { get; }
        public Guid FavoriteCoffeMachineId { get; }
    }
}