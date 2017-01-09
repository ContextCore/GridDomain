using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Commands;
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


        [Test]
        public void Given_reserving_state_When_order_item_added_Then_reserve_stock_command_is_issued()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Reserving),
                                               c => c.Without(d => d.ReserveId));

            scenario.GivenState(sagaId, state)
                    .When(new StockReserved(state.StockId, 
                                            state.ReserveId,
                                            DateTime.UtcNow.AddDays(1),
                                            state.Quantity)
                                   .CloneWithSaga(sagaId))

                    .Then(new CalculateOrderTotalCommand(state.OrderId)
                                    .CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }

        [Test]
        public void Given_reserving_state_When_order_total_calculated_Then_reserve_stock_command_is_issued()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Reserving),
                                               c => c.Without(d => d.ReserveId));

            var totalPrice = new Money(100);

            scenario.GivenState(sagaId, state)
                    .When(new OrderTotalCalculated(state.OrderId,totalPrice)
                                   .CloneWithSaga(sagaId))
                    .Then(new PayForOrderCommand(state.AccountId, totalPrice, state.OrderId)
                                   .CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }


        [Test]
        public void Given_reserving_state_When_order_total_calculated_and__order_item_added_Then_state_is_changed()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Reserving));

            var totalPrice = new Money(100);

            scenario.GivenState(sagaId, state)
                    .When(new OrderTotalCalculated(state.OrderId, totalPrice)
                                   .CloneWithSaga(sagaId),
                          new StockReserved(state.StockId,
                                   state.ReserveId,
                                   DateTime.UtcNow.AddDays(1),
                                   state.Quantity)
                          .CloneWithSaga(sagaId))
                    .Then(new PayForOrderCommand(state.AccountId, totalPrice, state.OrderId)
                                   .CloneWithSaga(sagaId),
                          new CalculateOrderTotalCommand(state.OrderId)
                                   .CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Paying));
        }

        [Test]
        public void Given_paying_state_When_account_withdrawal_for_bad_order_Then_state_is_not_changed()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Paying));

            scenario.GivenState(sagaId, state)
                    .When(new AccountWithdrawal(state.AccountId, Guid.NewGuid(),new Money(100))
                                   .CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Paying));
        }


        [Test]
        public void Given_paying_state_When_account_withdrawal_for_our_order_Then_state_is_taking_stock()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Paying));

            scenario.GivenState(sagaId, state)
                    .When(new AccountWithdrawal(state.AccountId, state.ReserveId, new Money(100))
                                   .CloneWithSaga(sagaId))
                     .Then(new TakeReservedStockCommand(state.StockId, state.ReserveId))
                     .Run()
                     .CheckProducedCommands()
                     .CheckOnlyStateNameChanged(nameof(BuyNow.TakingStock));
        }

        [Test]
        public void Given_taking_stock_state_When_stockReserveTaken_Then_complete_order_and_complete_user_pending_order_commands_are_issued()
        {
            var scenario = NewScenario();
            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.TakingStock));

            scenario.GivenState(sagaId, state)
                    .When(new StockReserveTaken(state.StockId, state.ReserveId).CloneWithSaga(sagaId))
                    .Then(new CompleteOrderCommand(state.OrderId).CloneWithSaga(sagaId),
                           new CompletePendingOrderCommand(state.UserId,state.OrderId).CloneWithSaga(sagaId))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Final));
        }
    }
}
