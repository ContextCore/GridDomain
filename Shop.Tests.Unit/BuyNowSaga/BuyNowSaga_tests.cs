using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
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

            var scenario = SagaScenario<BuyNow, BuyNowData, BuyNowSagaFactory>.New(BuyNow.Descriptor,factory);

            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sagaId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 1;
            var stockId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            scenario.When(new SkuPurchaseOrdered(userId,
                                                 skuId,
                                                 quantity,
                                                 orderId,
                                                 stockId,
                                                 accountId)
                               .CloneWithSaga(sagaId))
                     .Then(new CreateOrderCommand(orderId,userId))
                     .Run()
                     .Check();

            Assert.IsNotNull(scenario.SagaInstance.Data);
            Assert.AreEqual(sagaId,scenario.SagaInstance.Data.Id);

            var sagaState = scenario.SagaInstance.Data.Data;

            Assert.AreEqual(nameof(BuyNow.CreatingOrder), sagaState.CurrentStateName);
            Assert.AreEqual(accountId, sagaState.AccountId);
            Assert.AreEqual(orderId, sagaState.OrderId);
            Assert.AreEqual(quantity, sagaState.Quantity);
            Assert.AreEqual(userId, sagaState.ReserveId);
            Assert.AreEqual(skuId, sagaState.SkuId);
            Assert.AreEqual(stockId, sagaState.StockId);
            Assert.AreEqual(userId, sagaState.UserId);
        }
    }
}
