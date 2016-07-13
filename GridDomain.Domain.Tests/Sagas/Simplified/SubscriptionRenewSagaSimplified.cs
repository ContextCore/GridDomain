using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NEventStore;

namespace GridDomain.Tests.Sagas.Simplified
{
    class SubscriptionRenewProcess
    {
        public Guid BusinessId { get; set; }
        public Guid SubscriptionId { get; set; }
    }


    class SubscriptionRenewSagaSimplified : AutomatonymousStateMachine<SubscriptionRenewProcess>
    {

        private List<ICommand> _commandsToDispatch = new List<ICommand>(); 
        public void Dispatch(ICommand cmd)
        {
            _commandsToDispatch.Add(cmd);
        }
        public SubscriptionRenewSagaSimplified()
        {
            During(SubscriptionSet,
                When(SubscriptionExpired).Then(context =>
                {
                    context.Instance.SubscriptionId = context.Data.SourceId;
                    Dispatch(new PayForSubscriptionCommand(context.Data));
                })
                .TransitionTo(PayingForSubscription));

            During(PayingForSubscription, 
               When(NotEnoughFunds)
                 .Then(context => Dispatch(new ChangeSubscriptionCommand(context.Data)))
                 .TransitionTo(ChangingSubscription),
               When(SubscriptionPaid)
                 .TransitionTo(SubscriptionSet));

            During(ChangingSubscription,
                 When(SubscriptionChanged).TransitionTo(SubscriptionSet));
        }


       // = RegisterEvent<SubscriptionExpiredEvent>(SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers.PayForSubscription);
       // var remainSubscriptionTrigger = RegisterEvent<SubscriptionPaidEvent>(SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers.RemainSubscription);
       // var changeSubscriptionTrigger = RegisterEvent<SubscriptionChangedEvent>(SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers.ChangeSubscription);
       // var revokeSubscriptionTrigger = RegisterEvent<NotEnoughFundsFailure>(SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers.RevokeSubscription);

        public Event<SubscriptionExpiredEvent> SubscriptionExpired; //Trigger.SubscriptionExpired;
        public Event<SubscriptionPaidEvent>    SubscriptionPaid;    //Trigger.SubscriptionPaid;
        public Event<SubscriptionChangedEvent> SubscriptionChanged; //Trigger.SubscriptionChanged;
        public Event<NotEnoughFundsFailure>    NotEnoughFunds;      //Trigger.RevokeSubscription
        
        public State SubscriptionSet;       //State.SubscriptionSet
        public State PayingForSubscription; //State.OfferPaying
        public State ChangingSubscription;  //State.SubscriptionChanging
    }
}
