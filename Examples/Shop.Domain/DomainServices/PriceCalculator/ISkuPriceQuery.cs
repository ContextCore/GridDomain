using System;
using GridDomain.CQRS;
using NMoneys;

namespace Shop.Domain.DomainServices.PriceCalculator
{
    public interface ISkuPriceQuery : ISingleQuery<Guid, Money> {}
}