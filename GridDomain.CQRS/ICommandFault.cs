using System;

namespace GridDomain.CQRS
{
    public interface ICommandFault<T> where T : ICommand
    {
        T Command { get; }
        Exception Fault { get; }

        Guid SagaId { get; }
    }

    //public interface ICommandFault
    //{
    //    ICommand Command { get; }
    //    Exception Fault { get; }
    //}
}