using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using NMoneys;

namespace Shop.Domain.Aggregates.SkuAggregate.Commands
{
    public class CreateNewSkuCommand : Command
    {
        public string Name { get; }
        public string Article { get; }
        public Guid SkuId { get; }
        public Money Price { get; }

        public CreateNewSkuCommand(string name, string article,Guid skuId, Money price)
        {
            Name = name;
            Article = article;
            SkuId = skuId;
            Price = price;
        }
    }
}
