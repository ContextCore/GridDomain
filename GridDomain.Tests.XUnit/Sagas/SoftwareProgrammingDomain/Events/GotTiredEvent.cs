using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events
{
    public class GotTiredEvent: DomainEvent
    {
        public Guid PersonId => SourceId;
        public Guid LovelySofaId { get; }
        public Guid FavoriteCoffeMachineId { get; }
        public GotTiredEvent(Guid sourceId,
                             Guid lovelySofaId = default(Guid), 
                             Guid favoriteCoffeMachineId=default(Guid), 
                             Guid? sagaId = null,
                             DateTime? createdTime = null)
            : base(sourceId, createdTime,sagaId)
        {
            LovelySofaId = lovelySofaId;
            FavoriteCoffeMachineId = favoriteCoffeMachineId;
        }
    }
}