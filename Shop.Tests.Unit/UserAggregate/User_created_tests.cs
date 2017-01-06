using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Framework;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture.Kernel;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Events;

namespace Shop.Tests.Unit.UserAggregate
{


    [TestFixture]
    class User_created_tests
    {
        private readonly Guid _stockId = Guid.NewGuid();

        private AggregateScenario<User, UserCommandsHandler> NewScenario()
        {
            var stockProviderMoq = new Mock<IDefaultStockProvider>();
            stockProviderMoq.Setup(p => p.GetStockForSku(It.IsAny<Guid>())).Returns(_stockId);

            return AggregateScenario<User, UserCommandsHandler>.New(null,
                                                           new UserCommandsHandler(stockProviderMoq.Object));
        }
        [Test]
        public void When_user_created_by_command_Then_created_event_occures()
        {
            var cmd = new CreateUserCommand(Guid.NewGuid(),"testLogin",Guid.NewGuid());

            var scenario =  NewScenario()
                                   .When(cmd)
                                   .Then(new UserCreated(cmd.UserId, cmd.Login, cmd.AccountId));

            scenario.Run().Check();

            Assert.AreEqual(cmd.Login,scenario.Aggregate.Login);
            Assert.AreEqual(cmd.AccountId,scenario.Aggregate.Account);
            Assert.AreEqual(cmd.UserId,scenario.Aggregate.Id);
        }

        [Test]
        public void Given_user_created_When_buing_now_Then_pending_order_event_is_created()
        {
            var userId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 10;
            var account = Guid.NewGuid();

            var scenario = NewScenario()
                                   .Given(new UserCreated(userId, "testLogin", account))
                                   .When(new BuySkuNowCommand(userId, skuId,quantity))
                                   .Then(new SkuPurchaseOrdered(userId, skuId,10, Any.GUID, _stockId, account));

            scenario.Run().Check();

            //Aggregate state 
            var pendingOrderId = scenario.Aggregate.GetEvent<SkuPurchaseOrdered>().OrderId;
            var pendingOrder = scenario.Aggregate.PendingOrders[pendingOrderId];
            Assert.AreEqual(quantity, pendingOrder.Quantity);
            Assert.AreEqual(skuId, pendingOrder.SkuId);
            Assert.AreEqual(pendingOrderId, pendingOrder.Order);

        }

        [Test]
        public void Given_user_with_pending_order_When_cancel_order_Then_order_canceled_is_created()
        {
            var userId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 10;
            var orderId = Guid.NewGuid();
            var account = Guid.NewGuid();


            var scenario = NewScenario()
                                   .Given(new UserCreated(userId, "testLogin", account),
                                          new SkuPurchaseOrdered(userId,skuId,quantity,orderId,_stockId,account))
                                   .When(new CancelPendingOrderCommand(userId, orderId))
                                   .Then(new PendingOrderCanceled(userId,orderId));

            scenario.Run().Check();

            //pending order should be removed
            CollectionAssert.IsEmpty(scenario.Aggregate.PendingOrders);
        }

        [Test]
        public void Given_user_with_pending_order_When_complete_order_Then_order_completed_is_emitted()
        {
            var userId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 10;
            var orderId = Guid.NewGuid();
            var account = Guid.NewGuid();


            var scenario = NewScenario()
                                   .Given(new UserCreated(userId, "testLogin", account),
                                          new SkuPurchaseOrdered(userId, skuId, quantity, orderId, _stockId,account))
                                   .When(new CompletePendingOrderCommand(userId, orderId))
                                   .Then(new PendingOrderCompleted(userId, orderId));

            scenario.Run().Check();

            //pending order should be removed
            CollectionAssert.IsEmpty(scenario.Aggregate.PendingOrders);
        }
    }
}
