using System;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaState : ICloneable
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
    }
}