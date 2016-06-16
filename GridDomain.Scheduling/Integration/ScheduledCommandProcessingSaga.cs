using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Akka.Messages;
using NLog;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSaga : StateSaga<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
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
            var messageSent = RegisterEvent<ScheduledCommandProcessingStarted>(Transitions.SendMessage);
            var successTransition = RegisterEvent<ScheduledCommandSuccessfullyProcessed>(Transitions.Success);
            var failTransition = RegisterEvent<ScheduledCommandProcessingFailed>(Transitions.Failure);

            Machine
                .Configure(States.MessageSent)
                .OnEntryFrom(messageSent, e =>
                {
                    State.TaskId = e.Command.TaskId;
                    Dispatch(e.Command);
                    Dispatch(new CompleteJob(e.Command.TaskId, e.Command.Group));
                })
                .Permit(Transitions.Success, States.SuccessfullyProcessed)
                .Permit(Transitions.Failure, States.ProcessingFailure);

            Machine.Configure(States.SuccessfullyProcessed).OnEntryFrom(successTransition, @event =>
             {
                 _log.Error($"saga processed {State.TaskId}");
             });

            Machine.Configure(States.ProcessingFailure).OnEntryFrom(failTransition, @event =>
            {
                _log.Error(@event.Exception, $"saga failure {State.TaskId}");
            });
        }

        public static ISagaDescriptor SagaDescriptor => new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(Guid.Empty));
    }
}