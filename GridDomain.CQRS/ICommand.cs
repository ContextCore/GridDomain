using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommand : IHaveId
    {
        string ProcessId { get; }
        string AggregateId { get; }
        string AggregateType { get; }
    }
}