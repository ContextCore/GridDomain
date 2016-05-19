using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.CQRS;
using Stateless;

namespace GridDomain.Balance.Domain
{
    public class BuySubscriptionCommand:Command
    {
        public Guid BusinessId;
        public Guid SubscriptionId;
        public DateTime ActivateDate;
    }

    public class BuySubscriptionSaga: SagaBase<object>
    {
        public enum State
        {
            SubscriptionExist,
            ChargingBalance,
            BalanceCharged,
            ChanginSubscription
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

        public StateMachine<State, Transitions> Machine;
        public BuySubscriptionSaga()
        {
            Machine = new StateMachine<State, Transitions>(State.SubscriptionExist);
            Machine.Configure(State.SubscriptionExist)
                   .Permit(Transitions.ChargeForSubscription, State.ChargingBalance);

            Machine.Configure(State.ChargingBalance)
                   .Permit(Transitions.CharginFailed, State.SubscriptionExist)
                   .Permit(Transitions.ChargeSucceeded, State.BalanceCharged);

            Machine.Configure(State.BalanceCharged)
                   .Permit(Transitions.SetNewSubscription, State.ChanginSubscription);

            Machine.Configure(State.ChanginSubscription)
                   .Permit(Transitions.SubscriptionSet, State.SubscriptionExist)
                   .Permit(Transitions.SetFailed, State.BalanceCharged);
        }


    }
}
