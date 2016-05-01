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

        public enum Transition
        {
            ChargeForSubscription,
            CharginFailed,
            ChargeSucceeded,
            SetNewSubscription,
            SetFailed,
            SubscriptionSet
        }

        public StateMachine<State, Transition> Machine;
        public BuySubscriptionSaga()
        {
            Machine = new StateMachine<State, Transition>(State.SubscriptionExist);
            Machine.Configure(State.SubscriptionExist)
                   .Permit(Transition.ChargeForSubscription, State.ChargingBalance);

            Machine.Configure(State.ChargingBalance)
                   .Permit(Transition.CharginFailed, State.SubscriptionExist)
                   .Permit(Transition.ChargeSucceeded, State.BalanceCharged);

            Machine.Configure(State.BalanceCharged)
                   .Permit(Transition.SetNewSubscription, State.ChanginSubscription);

            Machine.Configure(State.ChanginSubscription)
                   .Permit(Transition.SubscriptionSet, State.SubscriptionExist)
                   .Permit(Transition.SetFailed, State.BalanceCharged);
        }


    }
}
