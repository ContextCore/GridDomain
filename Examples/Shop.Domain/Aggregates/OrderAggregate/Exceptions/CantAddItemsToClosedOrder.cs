using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.OrderAggregate.Exceptions
{
    public class CantAddItemsToClosedOrder : DomainException {}
}