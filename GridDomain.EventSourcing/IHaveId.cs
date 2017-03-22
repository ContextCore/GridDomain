using System;

namespace GridDomain.EventSourcing
{
    public interface IHaveId
    {
        Guid Id { get; }
    }
}