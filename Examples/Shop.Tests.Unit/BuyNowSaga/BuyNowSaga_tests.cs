using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using NMoneys;
using Ploeh.AutoFixture;
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
using Shop.Tests.Unit.DomainServices;
using Xunit;

namespace Shop.Tests.Unit.BuyNowSaga
{
   
    public class BuyNowSaga_tests
    {
        private readonly InMemoryPriceCalculator _inMemoryPriceCalculator;
        private readonly BuyNowSagaFactory _buyNowSagaFactory;

        public BuyNowSaga_tests()
        {
            _inMemoryPriceCalculator = new InMemoryPriceCalculator();
            _buyNowSagaFactory = new BuyNowSagaFactory(_inMemoryPriceCalculator, Log.Logger);
        }

        [Fact]
        public async Task Given_adding_order_items_state_When_order_item_added_Then_reserve_stock_command_is_issued()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);
            var state = scenario.NewState(nameof(BuyNow.AddingOrderItems), c => c.Without(d => d.ReserveId));

            _inMemoryPriceCalculator.Add(state.SkuId, new Money(100));

            await scenario.Given(state)
                                    .When(
                                          new ItemAdded(state.OrderId,
                                                        state.SkuId,
                                                        state.Quantity,
                                                        await _inMemoryPriceCalculator.CalculatePrice(state.SkuId, state.Quantity),
                                                        1))
                                    .Then(new ReserveStockCommand(state.StockId, state.UserId, state.Quantity))
                                    .Run()
                                    .CheckProducedCommands()
                                    .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }

       [Fact]
        public async Task Given_creating_order_state_When_order_created_Then_add_items_to_order_command_is_issued()
        {
            var calculator = new InMemoryPriceCalculator();
            var sagaFactory = new BuyNowSagaFactory(calculator, Log.Logger);
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, sagaFactory);

            var state = scenario.NewState(nameof(BuyNow.CreatingOrder), c => c.Without(d => d.ReserveId));

            calculator.Add(state.SkuId, new Money(100));

            await scenario.Given(state)
                          .When(new OrderCreated(state.OrderId, 123, state.UserId, OrderStatus.Created))
                          .Then(
                                new AddItemToOrderCommand(state.OrderId,
                                                          state.SkuId,
                                                          state.Quantity,
                                                          await calculator.CalculatePrice(state.SkuId, state.Quantity)))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.AddingOrderItems));
        }

       [Fact]
        public async Task Given_paying_state_When_account_withdrawal_for_bad_order_Then_state_is_not_changed()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);

            var state = scenario.NewState(nameof(BuyNow.Paying));

            await scenario.Given(state)
                    .When(new AccountWithdrawal(state.AccountId, Guid.NewGuid(), new Money(100)))
                    .Run()
                    .CheckProducedCommands()
                    .CheckOnlyStateNameChanged(nameof(BuyNow.Paying));
        }

       [Fact]
        public async Task Given_paying_state_When_account_withdrawal_for_our_order_Then_state_is_taking_stock()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);

            var state = scenario.NewState(nameof(BuyNow.Paying));

            await scenario.Given(state)
                          .When(new AccountWithdrawal(state.AccountId, state.OrderId, new Money(100)))
                          .Then(new TakeReservedStockCommand(state.StockId, state.ReserveId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.TakingStock));
        }

       [Fact]
        public async Task Given_reserving_state_When_order_item_added_Then_reserve_stock_command_is_issued()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);
            var state = scenario.NewState(nameof(BuyNow.Reserving), c => c.Without(d => d.ReserveId));

            await scenario.Given(state)
                          .When(new StockReserved(state.StockId, state.ReserveId, DateTime.UtcNow.AddDays(1), state.Quantity))
                          .Then(new CalculateOrderTotalCommand(state.OrderId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckStateName(nameof(BuyNow.Reserving));
        }

       [Fact]
        public async Task Given_reserving_state_When_order_total_calculated_and__order_item_added_Then_state_is_changed()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);
            var state = scenario.NewState(nameof(BuyNow.Reserving), c => c.Without(s => s.OrderWarReservedStatus));
            var totalPrice = new Money(100);

            await scenario.Given(state)
                          .When(new OrderTotalCalculated(state.OrderId, totalPrice),
                                new StockReserved(state.StockId, state.ReserveId, DateTime.UtcNow.AddDays(1), state.Quantity))
                          .Then(new PayForOrderCommand(state.AccountId, totalPrice, state.OrderId),
                                new CalculateOrderTotalCommand(state.OrderId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckStateName(nameof(BuyNow.Paying));
        }

       [Fact]
        public async Task Given_reserving_state_When_order_total_calculated_Then_reserve_stock_command_is_issued_And_state_is_not_changed()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);
            var state = scenario.NewState(nameof(BuyNow.Reserving), c => c.Without(d => d.ReserveId));

            var totalPrice = new Money(100);

            await scenario.Given(state)
                          .When(new OrderTotalCalculated(state.OrderId, totalPrice))
                          .Then(new PayForOrderCommand(state.AccountId, totalPrice, state.OrderId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Reserving));
        }

       [Fact]
        public async Task Given_sku_purchase_ordered_Then_buy_now_saga_is_created_and_create_order_command_issued()
        {
            SkuPurchaseOrdered ordered = new Fixture().Create<SkuPurchaseOrdered>();

            await SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory)
                              .When(ordered)
                              .Then(new CreateOrderCommand(ordered.OrderId, ordered.SourceId))
                              .Run()
                              .CheckProducedCommands(Compare.Ignore(nameof(ICommand.SagaId), 
                                                                    nameof(Command.Time),
                                                                    nameof(Command.Id)));

            //igoring saga id as it will be unique created on saga creation from event
        }

       [Fact]
        public async Task Given_taking_stock_state_When_stockReserveTaken_Then_complete_order_and_complete_user_pending_order_commands_are_issued()
        {
            var scenario = SagaScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, _buyNowSagaFactory);
            var state = scenario.NewState(nameof(BuyNow.TakingStock));

            await scenario.Given(state)
                          .When(new StockReserveTaken(state.StockId, state.ReserveId))
                          .Then(new CompleteOrderCommand(state.OrderId),
                                new CompletePendingOrderCommand(state.UserId, state.OrderId))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.Final));
        }
    }
}