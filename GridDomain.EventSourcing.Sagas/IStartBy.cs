using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    internal interface IStartBy<T> : IHandler<T>
    {
    }
}