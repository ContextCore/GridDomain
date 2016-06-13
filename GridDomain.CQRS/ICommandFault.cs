using System;

namespace GridDomain.CQRS
{
    public interface ICommandFault<out T>: ICommandFault where T : ICommand
    {
        new T Command { get; }
    }

    public interface ICommandFault
    {
        ICommand Command { get; }
        Exception Fault { get; }
        Guid SagaId { get; }
    }
}