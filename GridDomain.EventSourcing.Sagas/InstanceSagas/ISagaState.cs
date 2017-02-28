using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public interface ISagaState
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
    }
}