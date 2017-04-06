using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommand : IHaveId
    {
        Guid SagaId { get; }
        Guid AggregateId { get; }
    }
}