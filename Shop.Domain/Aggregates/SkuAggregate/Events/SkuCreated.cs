using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuAggregate.Events
{
    public class SkuCreated : DomainEvent
    {
        public string Article { get; }
        public int Number { get; }

        public string Name { get; }

        public SkuCreated(Guid sourceId, string name, string article, int number) : base(sourceId)
        {
            Article = article;
            Number = number;
            Name = name;

        }
    }
}