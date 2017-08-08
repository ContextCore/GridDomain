using System;

namespace GridDomain.EventSourcing
{
    public interface IHaveProcessId
    {
        Guid ProcessId { get; }
    }
}