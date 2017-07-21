using System;

namespace GridDomain.Processes
{
    public interface IProcessState : ICloneable
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
    }
}