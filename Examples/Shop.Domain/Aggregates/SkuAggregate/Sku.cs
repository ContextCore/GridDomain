using System;
using GridDomain.EventSourcing;
using NMoneys;
using Shop.Domain.Aggregates.SkuAggregate.Events;

namespace Shop.Domain.Aggregates.SkuAggregate
{
    public class Sku : Aggregate
    {
        private Sku(Guid id) : base(id)
        {
            Apply<SkuCreated>(e =>
                              {
                                  Id = e.SourceId;
                                  Name = e.Name;
                                  Article = e.Article;
                                  Number = e.Number;
                                  Price = e.Price;
                              });
        }

        public Sku(Guid id, string name, string article, int number, Money price) : this(id)
        {
            Produce(new SkuCreated(id, name, article, number, price));
        }

        public string Name { get; private set; }
        public string Article { get; private set; }
        public int Number { get; private set; }
        public Money Price { get; private set; }
    }
}