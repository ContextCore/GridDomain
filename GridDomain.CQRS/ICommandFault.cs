using System;

namespace GridDomain.CQRS
{
    public interface ICommandFault<out T>: ICommandFault where T : ICommand
    {
        new T Command { get; }
    }

    public interface ICommandFault
    {
        Guid Id { get; }
        ICommand Command { get; }
        Exception Exception { get; }
        Guid SagaId { get; }
    }
}