using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.SkuAggregate.Commands
{
    public class CreateNewSkuCommand : Command
    {
        public string Name { get; }
        public string Article { get; }
        public Guid SkuId { get; }

        public CreateNewSkuCommand(string name, string article,Guid skuId)
        {
            Name = name;
            Article = article;
            SkuId = skuId;
        }
    }
}
