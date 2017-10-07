
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IRouteEvents
    {
        void Dispatch(IAggregate aggregate, DomainEvent eventMessage);
    }

    public interface IRouteCommands
    {
        Task Dispatch(IAggregate aggregate, ICommand eventMessage);
    }
}