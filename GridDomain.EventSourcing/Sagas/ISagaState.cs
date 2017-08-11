using System;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaState
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
        ISagaState Clone();
    }
}