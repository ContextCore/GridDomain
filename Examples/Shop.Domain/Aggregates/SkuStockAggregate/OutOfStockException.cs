using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class OutOfStockException : DomainException
    {
        public OutOfStockException(int requested, int remaining)
        {
            Requested = requested;
            Remaining = remaining;
        }

        public int Requested { get; }
        public int Remaining { get; }
    }
}