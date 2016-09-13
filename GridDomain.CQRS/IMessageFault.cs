using System;

namespace GridDomain.CQRS
{
    public interface IMessageFault
    {
        object Message { get; }
     //   Guid Id { get; }
        Exception Exception { get; }
        Guid SagaId { get; }
        DateTime OccuredTime { get; }
        Type Processor { get; }
    }
}