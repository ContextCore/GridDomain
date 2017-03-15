using System;
using System.Runtime.Serialization;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public interface ISagaState : ICloneable
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
    }
}