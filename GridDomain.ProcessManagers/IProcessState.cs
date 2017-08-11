using System;

namespace GridDomain.ProcessManagers
{
    public interface IProcessState 
    {
        Guid Id { get; }
        string CurrentStateName { get; set; }
        IProcessState Clone();
    }
}