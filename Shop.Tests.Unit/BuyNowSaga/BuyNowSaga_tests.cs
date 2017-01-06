using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices;
using Shop.Domain.Sagas;

namespace Shop.Tests.Unit.BuyNowSaga
{
    [TestFixture]
    class BuyNowSaga_tests
    {
        [Test]
        public void Given_sku_purchase_ordered_Then_buy_now_saga_is_created()
        {
            var factory = new BuyNowSagaFactory(new InMemoryPriceCalculator());


            dynamic scenario=null;

            scenario.Given(new SkuPurchaseOrdered(Guid.NewGuid(), 
                                                  Guid.NewGuid(), 
                                                  1, 
                                                  Guid.NewGuid(), 
                                                  Guid.NewGuid(),
                                                  Guid.NewGuid()));

        }
    }
}
