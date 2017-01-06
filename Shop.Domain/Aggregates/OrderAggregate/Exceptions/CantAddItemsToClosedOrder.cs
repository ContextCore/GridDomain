using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    public class CantAddItemsToClosedOrder : DomainException
    {
    }
}