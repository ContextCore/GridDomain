using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events
{
    public class GotTiredEvent : DomainEvent
    {
        public GotTiredEvent(string sourceId,
                             string lovelySofaId = null,
                             string favoriteCoffeMachineId = null,
                             string processId = null,
                             DateTime? createdTime = null) : base(sourceId, processId, createdTime: createdTime)
        {
            LovelySofaId = lovelySofaId;
            FavoriteCoffeMachineId = favoriteCoffeMachineId;
        }
        
        public string PersonId => SourceId;
        public string LovelySofaId { get; }
        public string FavoriteCoffeMachineId { get; }
    }
}