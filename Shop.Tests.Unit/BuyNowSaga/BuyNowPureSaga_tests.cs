using System;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Tests;
using NMoneys;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.DomainServices;
using Shop.Domain.Sagas;

namespace Shop.Tests.Unit.BuyNowSaga
{
    [TestFixture]
    class BuyNowPureSaga_tests
    {
        private readonly InMemoryPriceCalculator _inMemoryPriceCalculator = new InMemoryPriceCalculator();

        [Test]

        public async Task BuyNow_Pure_Test()
        {
            var saga = new BuyNowPure();
            var state = new BuyNowData("");
            state.CurrentStateName = nameof(BuyNow.Reserving);

            var orderTotalCalculated = new OrderTotalCalculated(state.OrderId, new Money(100));

            var stockReserved = new StockReserved(state.StockId,
                                                  state.ReserveId,
                                                  DateTime.UtcNow.AddDays(1),
                                                  state.Quantity);


            await saga.RaiseEvent(state, saga.StockReserved, stockReserved);
            await saga.RaiseEvent(state, saga.OrderFinilized, orderTotalCalculated);

            Assert.AreEqual(nameof(saga.Paying), state.CurrentStateName);
        }

        [Test]

        public async Task BuyNow_Test()
        {
            var saga = new BuyNow(null);
            var state = new BuyNowData("");
            state.CurrentStateName = nameof(BuyNow.Reserving);

            var orderTotalCalculated = new OrderTotalCalculated(state.OrderId, new Money(100));

            var stockReserved = new StockReserved(state.StockId,
                                                  state.ReserveId,
                                                  DateTime.UtcNow.AddDays(1),
                                                  state.Quantity);


            await saga.RaiseEvent(state, saga.StockReserved, stockReserved);
            await saga.RaiseEvent(state, saga.OrderFinilized, orderTotalCalculated);

            Assert.AreEqual(nameof(saga.Paying), state.CurrentStateName);
        }
    }
}