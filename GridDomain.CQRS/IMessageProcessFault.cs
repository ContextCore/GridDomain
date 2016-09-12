using System;

namespace GridDomain.CQRS
{
    public interface IMessageProcessFault
    {
        object Message { get; }
        Guid Id { get; }
        Exception Exception { get; }
        Guid SagaId { get; }
        DateTime OccuredTime { get; }
    }
    public interface IMessageProcessFault<T> : IMessageProcessFault
    {
        new T Message { get; }
    }
}