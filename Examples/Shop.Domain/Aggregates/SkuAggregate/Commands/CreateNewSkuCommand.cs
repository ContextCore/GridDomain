using System;
using GridDomain.CQRS;
using NMoneys;

namespace Shop.Domain.Aggregates.SkuAggregate.Commands
{
    public class CreateNewSkuCommand : Command
    {
        public CreateNewSkuCommand(string name, string article, Guid skuId, Money price) : base(skuId)
        {
            Name = name;
            Article = article;
            Price = price;
        }

        public string Name { get; }
        public string Article { get; }
        public Guid SkuId => AggregateId;
        public Money Price { get; }
    }
}