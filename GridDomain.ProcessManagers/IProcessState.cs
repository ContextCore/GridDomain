using System;

namespace GridDomain.ProcessManagers
{
    public interface IProcessState : ICloneable
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
    }
}