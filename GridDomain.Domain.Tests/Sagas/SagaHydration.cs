using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using GridDomain.EventSourcing;
using Stateless;

namespace GridDomain.Domain.Tests.Sagas
{

    public enum TestState
    {
        SubscriptionSet,
        PaymentRequested,
        SubscriptionChanging
    }

    public enum TestTransition
    {
        OrderSubscription,
        PayForSubscription,
        SetSubscription
    }

    public class PayForSubscriptionCommand : Command
    {
        public Guid SubscriptionId;
    }

    public class ChangeSubscription : Command
    {
        public Guid SubscriptionId;
        public Guid BusinessId;
    }

    public class SubscriptionPaidEvent : DomainEvent
    {
        public SubscriptionPaidEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }

    public class SubscriptionChangedEvent : DomainEvent
    {
        public SubscriptionChangedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }

    public class SubscriptionOrderedEvent : DomainEvent
    {
        public SubscriptionOrderedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }

    class TestSaga : StateSaga<TestState, TestTransition>
    {
        public TestSaga(Guid id, SagaStateAggregate<TestState, TestTransition> state) : base(id, state)
        {
        }

        protected override void InitializeMachine(StateMachine<TestState, TestTransition> machine)
        {
            machine.Configure(TestState.SubscriptionSet)
                   .Permit(TestTransition.OrderSubscription, TestState.PaymentRequested);

            machine.Configure(TestState.PaymentRequested)
                   .Permit(TestTransition.PayForSubscription, Test)
                   .OnEntry(() => Dispatch(new PayForSubscriptionCommand()));

        }
    }

    class SagaHydration : HydrateSpecification<>
    {
    }
}
