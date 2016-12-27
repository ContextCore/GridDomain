using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class NotEnoughMoneyException : DomainException
    {
        public NotEnoughMoneyException(string message):base(message)
        {
        }
    }
}