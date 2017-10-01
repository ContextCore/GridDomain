
namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IRouteEvents
    {
        void Dispatch(IAggregate aggregate, DomainEvent eventMessage);
    }
}