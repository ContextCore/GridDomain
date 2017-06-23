using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using KellermanSoftware.CompareNetObjects;
using NMoneys;
using Serilog;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.AccountAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.Sagas;
using Shop.Tests.Unit.XUnit.DomainServices;
using Xunit;

namespace Shop.Tests.Unit.XUnit.BuyNowSaga
{
   
    internal class BuyNowSaga_tests
    {
        private readonly InMemoryPriceCalculator _inMemoryPriceCalculator = new InMemoryPriceCalculator();

        private SagaScenario<BuyNow, BuyNowState, BuyNowSagaFactory> NewScenario()
        {
            var factory = new BuyNowSagaFactory(_inMemoryPriceCalculator, Log.Logger);
            var scenario = SagaScenario<BuyNow, BuyNowState, BuyNowSagaFactory>.New(BuyNow.Descriptor, factory);
            return scenario;
        }

       [Fact]
        public async Task Given_adding_order_items_state_When_order_item_added_Then_reserve_stock_command_is_issued()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.AddingOrderItems), c => c.Without(d => d.ReserveId));

            _inMemoryPriceCalculator.Add(state.SkuId, new Money(100));

            var res = await scenario.GivenState(sagaId, state)
                                    .When(
                                          new ItemAdded(state.OrderId,
                                                        state.SkuId,
                                                        state.Quantity,
                                                        await _inMemoryPriceCalculator.CalculatePrice(state.SkuId, state.Quantity),
                                                        1).CloneWithSaga(sagaId))
                                    .Then(new ReserveStockCommand(state.StockId, state.UserId, state.Quantity).CloneWithSaga(sagaId))
                                    .Run();

            res.CheckProducedCommands()
               .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }

       [Fact]
        public async Task Given_creating_order_state_When_order_created_Then_add_items_to_order_command_is_issued()
        {
            var scenario = NewScenario();
            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.CreatingOrder), c => c.Without(d => d.ReserveId));

            _inMemoryPriceCalculator.Add(state.SkuId, new Money(100));

            await scenario.GivenState(sagaId, state)
                          .When(new OrderCreated(state.OrderId, 123, state.UserId, OrderStatus.Created).CloneWithSaga(sagaId))
                          .Then(
                                new AddItemToOrderCommand(state.OrderId,
                                                          state.SkuId,
                                                          state.Quantity,
                                                          await _inMemoryPriceCalculator.CalculatePrice(state.SkuId, state.Quantity)).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.AddingOrderItems));
        }

       [Fact]
        public async Task Given_paying_state_When_account_withdrawal_for_bad_order_Then_state_is_not_changed()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Paying));

            await scenario.GivenState(sagaId, state)
                          .When(new AccountWithdrawal(state.AccountId, Guid.NewGuid(), new Money(100)).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Paying));
        }

       [Fact]
        public async Task Given_paying_state_When_account_withdrawal_for_our_order_Then_state_is_taking_stock()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Paying));

            await scenario.GivenState(sagaId, state)
                          .When(new AccountWithdrawal(state.AccountId, state.OrderId, new Money(100)).CloneWithSaga(sagaId))
                          .Then(new TakeReservedStockCommand(state.StockId, state.ReserveId).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.TakingStock));
        }

       [Fact]
        public async Task Given_reserving_state_When_order_item_added_Then_reserve_stock_command_is_issued()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Reserving), c => c.Without(d => d.ReserveId));

            await scenario.GivenState(sagaId, state)
                          .When(
                                new StockReserved(state.StockId, state.ReserveId, DateTime.UtcNow.AddDays(1), state.Quantity)
                                    .CloneWithSaga(sagaId))
                          .Then(new CalculateOrderTotalCommand(state.OrderId).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }

       [Fact]
        public async Task Given_reserving_state_When_order_total_calculated_and__order_item_added_Then_state_is_changed()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Reserving), c => c.Without(s => s.OrderWarReservedStatus));
            var totalPrice = new Money(100);

            await scenario.GivenState(sagaId, state)
                          .When(new OrderTotalCalculated(state.OrderId, totalPrice).CloneWithSaga(sagaId),
                                new StockReserved(state.StockId, state.ReserveId, DateTime.UtcNow.AddDays(1), state.Quantity)
                                    .CloneWithSaga(sagaId))
                          .Then(new PayForOrderCommand(state.AccountId, totalPrice, state.OrderId).CloneWithSaga(sagaId),
                                new CalculateOrderTotalCommand(state.OrderId).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Paying));
        }

       [Fact]
        public async Task Given_reserving_state_When_order_total_calculated_Then_reserve_stock_command_is_issued_And_state_is_not_changed()
        {
            var scenario = NewScenario();

            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.Reserving), c => c.Without(d => d.ReserveId));

            var totalPrice = new Money(100);

            await scenario.GivenState(sagaId, state)
                          .When(new OrderTotalCalculated(state.OrderId, totalPrice).CloneWithSaga(sagaId))
                          .Then(new PayForOrderCommand(state.AccountId, totalPrice, state.OrderId).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }

       [Fact]
        public async Task Given_sku_purchase_ordered_Then_buy_now_saga_is_created_and_create_order_command_issued()
        {
            var scenario = NewScenario();

            var state = scenario.GenerateState(nameof(BuyNow.CreatingOrder), c => c.Without(d => d.ReserveId));
            var sagaId = state.Id;

            var compare_ignore_complex_event_status = new CompareLogic
                                                      {
                                                          Config = new ComparisonConfig
                                                                   {
                                                                       MembersToIgnore = new List<string>
                                                                                         {
                                                                                             nameof(state.OrderWarReservedStatus)
                                                                                         }
                                                                   }
                                                      };

            await scenario.When(
                                new SkuPurchaseOrdered(state.UserId,
                                                       state.SkuId,
                                                       state.Quantity,
                                                       state.OrderId,
                                                       state.StockId,
                                                       state.AccountId).CloneWithSaga(sagaId))
                          .Then(new CreateOrderCommand(state.OrderId, state.UserId).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckProducedState(state, compare_ignore_complex_event_status);

            Assert.Equal(sagaId, scenario.Saga.State.Id);
        }

       [Fact]
        public async Task Given_taking_stock_state_When_stockReserveTaken_Then_complete_order_and_complete_user_pending_order_commands_are_issued()
        {
            var scenario = NewScenario();
            var sagaId = Guid.NewGuid();
            var state = scenario.GenerateState(nameof(BuyNow.TakingStock));

            await scenario.GivenState(sagaId, state)
                          .When(new StockReserveTaken(state.StockId, state.ReserveId).CloneWithSaga(sagaId))
                          .Then(new CompleteOrderCommand(state.OrderId).CloneWithSaga(sagaId),
                                new CompletePendingOrderCommand(state.UserId, state.OrderId).CloneWithSaga(sagaId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Final));
        }
    }
}