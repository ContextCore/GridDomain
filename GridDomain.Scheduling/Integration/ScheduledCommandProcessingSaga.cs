using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSaga : StateSaga<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();
        public enum States
        {
            Created,
            MessageSent,
            SuccessfullyProcessed,
            ProcessingFailure
        }

        public enum Transitions
        {
            SendMessage,
            Success,
            Failure
        }

        public override void Transit(object msg)
        {
            var fault = msg as ICommandFault;
            if (fault != null)
            {
                TransitState(fault);
                return;
            }
            var domainEvent = msg as DomainEvent;
            if (domainEvent != null && domainEvent.GetType() == State.SuccessEventType)
            {
                TransitState(domainEvent);
                return;
            }
            if (msg.GetType() == StartMessage)
            {
                base.Transit(msg);
            }
        }

        public ScheduledCommandProcessingSaga(ScheduledCommandProcessingSagaState state) : base(state)
        {
            Machine.Configure(States.Created)
                .Permit(Transitions.SendMessage, States.MessageSent)
                .OnExit(() => Dispatch(new CompleteJob(State.Key.Name, State.Key.Group)));

            var messageSent = RegisterEvent<ScheduledCommandProcessingStarted>(Transitions.SendMessage);
            var faultsTrigger = RegisterEvent<ICommandFault>(Transitions.Failure);
            var eventrTrigger = RegisterEvent<DomainEvent>(Transitions.Success);

            Machine
                .Configure(States.ProcessingFailure)
                .OnEntryFrom(faultsTrigger, fault =>
                {
                    _log.Error(fault.Exception, "Scheduled command processing failure");
                });

            Machine
                .Configure(States.SuccessfullyProcessed)
                .OnEntryFrom(eventrTrigger, domainEvent => { _log.Info($"Scheduled command successfully processed {domainEvent.SagaId}"); });

            Machine
                .Configure(States.MessageSent)
                .OnEntryFrom(messageSent, e =>
                {
                    Dispatch(e.Command);
                })
                .Permit(Transitions.Success, States.SuccessfullyProcessed)
                .Permit(Transitions.Failure, States.ProcessingFailure);
        }
        public static ISagaDescriptor SagaDescriptor => new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(Guid.Empty));
    }
}