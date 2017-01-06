using System;
using System.Collections.Generic;
using NMoneys;

namespace Shop.Domain.Sagas
{
    //must not change state after method invocation 
    public interface IPriceCalculator
    { 
        Money CalculatePrice(Guid skuId, int quantity);
    }
}