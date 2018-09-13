using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommand : IHaveId
    {
        string ProcessId { get; set; }
        string AggregateId { get; }
        string AggregateName { get; }
    }
}