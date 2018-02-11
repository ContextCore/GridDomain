using System;

namespace GridDomain.ProcessManagers
{
    public interface IProcessState 
    {
        string Id { get; }
        string CurrentStateName { get; set; }
        IProcessState Clone();
    }
}