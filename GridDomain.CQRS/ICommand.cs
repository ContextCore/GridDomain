using System;

namespace GridDomain.CQRS
{
    public interface ICommand
    {
        Guid Id { get; }
        Guid SagaId { get; }
        Guid AggregateId { get; }
    }
}