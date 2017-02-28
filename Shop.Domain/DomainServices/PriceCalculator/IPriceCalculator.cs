using System;
using System.Threading.Tasks;
using NMoneys;

namespace Shop.Domain.DomainServices.PriceCalculator
{
    //must not change state after method invocation 
    public interface IPriceCalculator
    {
        Task<Money> CalculatePrice(Guid skuId, int quantity);
    }
}