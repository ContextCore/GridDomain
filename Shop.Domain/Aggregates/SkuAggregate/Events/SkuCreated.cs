using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.SkuAggregate.Events
{
    public class SkuCreated : DomainEvent
    {
        public SkuCreated(Guid sourceId, string name, string article, int number, Money price) : base(sourceId)
        {
            Article = article;
            Number = number;
            Name = name;
            Price = price;
        }

        public Money Price { get; }

        public string Article { get; }
        public int Number { get; }

        public string Name { get; }
    }
}