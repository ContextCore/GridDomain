using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSaga : StateSaga<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions, ScheduledCommandProcessingSagaState, ScheduledMessageProcessingStarted>
    {
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

        public ScheduledCommandProcessingSaga(ScheduledCommandProcessingSagaState state) : base(state)
        {
            Machine.Configure(States.Created)
                .Permit(Transitions.SendMessage, States.MessageSent);
            var messageSent = RegisterEvent<ScheduledMessageProcessingStarted>(Transitions.SendMessage);
            RegisterEvent<ScheduledMessageProcessingStarted>(Transitions.SendMessage);

            Machine
                .Configure(States.MessageSent)
                .OnEntryFrom(messageSent, e =>
                {
                    Dispatch(e.Command);
                    Dispatch(new CompleteJob(e.Command.TaskId, e.Command.Group));
                })
                .Permit(Transitions.Success, States.SuccessfullyProcessed)
                .Permit(Transitions.Failure, States.ProcessingFailure);

            Machine.Configure(States.SuccessfullyProcessed).OnEntry(() =>
            {

            });

            Machine.Configure(States.ProcessingFailure).OnEntry(() =>
            {

            });
        }

        public static ISagaDescriptor SagaDescriptor => new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(Guid.Empty));
    }
}