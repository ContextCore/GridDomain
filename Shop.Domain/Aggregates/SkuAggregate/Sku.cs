using System;
using GridDomain.EventSourcing;
using Shop.Domain.Aggregates.SkuAggregate.Events;

namespace Shop.Domain.Aggregates.SkuAggregate
{
    public class Sku : Aggregate
    {
        public string Name { get; private set; }
        public string Article { get; private set; }
        public int Number { get; private set; }
        private Sku(Guid id) : base(id)
        {
            Apply<SkuCreated>(e =>
            {
                Id = e.SourceId;
                Name = e.Name;
                Article = e.Article;
                Number = e.Number;
            });
        }

        public Sku(Guid id, string name, string article, int number) : this(id)
        {
            RaiseEvent(new SkuCreated(id,name,article,number));
        }
    }
}