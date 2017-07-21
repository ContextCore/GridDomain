using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommand : IHaveId
    {
        Guid ProcessId { get; }
        Guid AggregateId { get; }
    }
}