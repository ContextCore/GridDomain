using System;

namespace GridDomain.EventSourcing
{
    public interface IHaveProcessId
    {
        string ProcessId { get; }
    }
}