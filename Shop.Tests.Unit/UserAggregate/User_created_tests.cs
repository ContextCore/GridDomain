using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Events;

namespace Shop.Tests.Unit.UserAggregate
{


    [TestFixture]
    class User_created_tests
    {
        [Test]
        public void When_user_created_by_command_Then_created_event_occures()
        {
            var cmd = new CreateUserCommand(Guid.NewGuid(),"testLogin",Guid.NewGuid());

            var scenario = Scenario.New<User, UserCommandsHandler>()
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

            var scenario = Scenario.New<User, UserCommandsHandler>()
                                   .Given(new UserCreated(userId, "testLogin", Guid.NewGuid()))
                                   .When(new BuySkuNowCommand(userId, skuId,quantity))
                                   .Then(new BuyNowOrderAdded(userId, skuId,10, Any.GUID));

            scenario.Run().Check();

            //Aggregate state 
            var pendingOrderId = scenario.Aggregate.GetEvent<BuyNowOrderAdded>().OrderId;
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

            var scenario = Scenario.New<User, UserCommandsHandler>()
                                   .Given(new UserCreated(userId, "testLogin", Guid.NewGuid()),
                                          new BuyNowOrderAdded(userId,skuId,quantity,orderId))
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

            var scenario = Scenario.New<User, UserCommandsHandler>()
                                   .Given(new UserCreated(userId, "testLogin", Guid.NewGuid()),
                                          new BuyNowOrderAdded(userId, skuId, quantity, orderId))
                                   .When(new CompletePendingOrderCommand(userId, orderId))
                                   .Then(new PendingOrderCompleted(userId, orderId));

            scenario.Run().Check();

            //pending order should be removed
            CollectionAssert.IsEmpty(scenario.Aggregate.PendingOrders);
        }
    }
}
