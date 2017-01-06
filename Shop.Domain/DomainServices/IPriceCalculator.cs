using System;
using NMoneys;

namespace Shop.Domain.DomainServices
{
    //must not change state after method invocation 
    public interface IPriceCalculator
    { 
        Money CalculatePrice(Guid skuId, int quantity);
    }
}