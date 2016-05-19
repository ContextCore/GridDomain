using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    interface IStartBy<T> : IHandler<T>
    {
        
    }
}