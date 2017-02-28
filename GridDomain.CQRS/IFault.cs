using System;

namespace GridDomain.CQRS
{
    /// <summary>
    ///     Represent information of error Exception occurred in Processor while handling Message
    ///     Typically Processors is type of domain-specific classes: sagas, aggregates, message handlers
    /// </summary>
    public interface IFault
    {
        object Message { get; }
        Exception Exception { get; }
        Guid SagaId { get; }
        DateTime OccuredTime { get; }
        Type Processor { get; }
    }

    public interface IFault<out T> : IFault
    {
        new T Message { get; }
    }
}