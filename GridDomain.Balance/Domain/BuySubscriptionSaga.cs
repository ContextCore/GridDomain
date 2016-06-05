using CommonDomain.Core;
using GridDomain.Balance.Domain.BusinessAggregate;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using Stateless;

namespace GridDomain.Balance.Domain
{
    public class BuySubscriptionSaga : StateSaga<BuySubscriptionSaga.State, 
                                                 BuySubscriptionSaga.Transitions, 
                                                 SubscriptionOrderedEvent>
    {
        public enum State
        {
            SubscriptionExist,
            ChargingBalance,
            BalanceCharged,
            ChangingSubscription
        }

        public enum Transitions
        {
            ChargeForSubscription,
            CharginFailed,
            ChargeSucceeded,
            SetNewSubscription,
            SetFailed,
            SubscriptionSet
        }

        public BuySubscriptionSaga(SagaStateAggregate<State,
                                                     Transitions> machine ):base(machine)
        {
            Machine.Configure(State.SubscriptionExist)
                .Permit(Transitions.ChargeForSubscription, State.ChargingBalance);

            Machine.Configure(State.ChargingBalance)
                .Permit(Transitions.CharginFailed, State.SubscriptionExist)
                .Permit(Transitions.ChargeSucceeded, State.BalanceCharged);

            Machine.Configure(State.BalanceCharged)
                .Permit(Transitions.SetNewSubscription, State.ChangingSubscription);

            Machine.Configure(State.ChangingSubscription)
                .Permit(Transitions.SubscriptionSet, State.SubscriptionExist)
                .Permit(Transitions.SetFailed, State.BalanceCharged);
        }
    }
}