using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices;
using Shop.Domain.Sagas;

namespace Shop.Tests.Unit.BuyNowSaga
{
    [TestFixture]
    class BuyNowSaga_tests
    {
        private readonly InMemoryPriceCalculator _inMemoryPriceCalculator = new InMemoryPriceCalculator();

        [Test]
        public void Given_sku_purchase_ordered_Then_buy_now_saga_is_created_and_create_order_command_issued()
        {
            var scenario = NewScenario();
            var sagaId = Guid.NewGuid();

            var state = scenario.GenerateState(nameof(BuyNow.CreatingOrder),
                                               c => c.Without(d => d.ReserveId));

            scenario.When(new SkuPurchaseOrdered(state.UserId,
                                                 state.SkuId,
                                                 state.Quantity,
                                                 state.OrderId,
                                                 state.StockId,
                                                 state.AccountId).CloneWithSaga(sagaId))
                    .Then(new CreateOrderCommand(state.OrderId, state.UserId).CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckProducedState(state);

            Assert.AreEqual(sagaId, scenario.SagaInstance.Data.Id);
        }

        [Test]
        public void Given_creating_order_state_When_order_created_Then_add_items_to_order_command_is_issued()
        {
            var scenario = NewScenario();
            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.CreatingOrder),
                                               c => c.Without(d => d.ReserveId));

            _inMemoryPriceCalculator.Add(state.SkuId, new Money(100));

            scenario.GivenState(sagaId, state)
                    .When(new OrderCreated(state.OrderId,
                                           123,
                                           state.UserId,
                                           OrderStatus.Created).CloneWithSaga(sagaId))
                    .Then(new AddItemToOrderCommand(state.OrderId,
                                                    state.SkuId,
                                                    state.Quantity,
                                                    _inMemoryPriceCalculator.CalculatePrice(state.SkuId, state.Quantity))
                                                .CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.AddingOrderItems));
        }

        private SagaScenario<BuyNow, BuyNowData, BuyNowSagaFactory> NewScenario()
        {
            var factory = new BuyNowSagaFactory(_inMemoryPriceCalculator);
            var scenario = SagaScenario<BuyNow, BuyNowData, BuyNowSagaFactory>.New(BuyNow.Descriptor, factory);
            return scenario;
        }

        [Test]
        public void Given_adding_order_items_state_When_order_item_added_Then_reserve_stock_command_is_issued()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.AddingOrderItems),
                                               c => c.Without(d => d.ReserveId));

            _inMemoryPriceCalculator.Add(state.SkuId, new Money(100));

            scenario.GivenState(sagaId, state)
                    .When(new ItemAdded(state.OrderId,
                                        state.SkuId,
                                        state.Quantity,
                                       _inMemoryPriceCalculator.CalculatePrice(state.SkuId,state.Quantity),
                                        1).CloneWithSaga(sagaId))

                    .Then(new ReserveStockCommand(state.StockId,
                                                  state.UserId,
                                                  state.Quantity)
                                                .CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }
    }
}
